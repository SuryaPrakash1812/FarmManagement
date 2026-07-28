using FarmManagement.Api.Data;
using FarmManagement.Api.Domain;
using FarmManagement.Api.Dtos;
using FarmManagement.Api.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/expenses")]
public sealed class ExpensesController : ControllerBase
{
    private readonly FarmDbContext _db;
    public ExpensesController(FarmDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<PagedResult<ExpenseDto>>> Get([FromQuery] string? search, [FromQuery] Guid? personId, [FromQuery] DateOnly? from, [FromQuery] DateOnly? to, [FromQuery] bool? recurring, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var query = _db.Expenses.Include(x => x.PaidByUser).Include(x => x.Splits).ThenInclude(s => s.User).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.Category.Contains(search) || x.PaymentMethod.Contains(search));
        if (personId.HasValue) query = query.Where(x => x.PaidByUserId == personId.Value || x.Splits.Any(s => s.UserId == personId.Value));
        if (from.HasValue) query = query.Where(x => x.Date >= from.Value);
        if (to.HasValue) query = query.Where(x => x.Date <= to.Value);
        if (recurring.HasValue) query = query.Where(x => x.IsRecurring == recurring.Value);
        var total = await query.CountAsync(ct);
        var entities = await query.OrderByDescending(x => x.Date).ThenByDescending(x => x.CreatedAtUtc).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return Ok(new PagedResult<ExpenseDto>(entities.Select(EntityMapper.ToDto).ToList(), total, page, pageSize));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ExpenseDto>> GetById(Guid id, CancellationToken ct)
    {
        var entity = await _db.Expenses.Include(x => x.PaidByUser).Include(x => x.Splits).ThenInclude(s => s.User).FirstOrDefaultAsync(x => x.Id == id, ct);
        return entity is null ? NotFound() : Ok(EntityMapper.ToDto(entity));
    }

    [HttpPost]
    public async Task<ActionResult<ExpenseDto>> Create(UpsertExpenseRequest request, CancellationToken ct)
    {
        var entity = EntityMapper.ToEntity(request);
        ApplySplits(entity, request);
        _db.Expenses.Add(entity);
        await _db.SaveChangesAsync(ct);
        await ReloadNavigationsAsync(entity, ct);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, EntityMapper.ToDto(entity));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ExpenseDto>> Update(Guid id, UpsertExpenseRequest request, CancellationToken ct)
    {
        var entity = await _db.Expenses.Include(x => x.Splits).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return NotFound();
        EntityMapper.Apply(request, entity);
        _db.ExpenseSplits.RemoveRange(entity.Splits);
        entity.Splits.Clear();
        ApplySplits(entity, request);
        await _db.SaveChangesAsync(ct);
        await ReloadNavigationsAsync(entity, ct);
        return Ok(EntityMapper.ToDto(entity));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var entity = await _db.Expenses.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return NotFound();
        entity.IsDeleted = true;
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpGet("balances")]
    public async Task<ActionResult<IEnumerable<ExpenseBalanceDto>>> Balances(CancellationToken ct) => Ok(await ComputeBalancesAsync(ct));

    [HttpGet("settle-suggestions")]
    public async Task<ActionResult<IEnumerable<SettleSuggestionDto>>> SettleSuggestions(CancellationToken ct)
    {
        var balances = await ComputeBalancesAsync(ct);
        var creditors = balances.Where(b => b.NetBalance > 0.01m).OrderByDescending(b => b.NetBalance).Select(b => (b.UserId, b.FullName, Amount: b.NetBalance)).ToList();
        var debtors = balances.Where(b => b.NetBalance < -0.01m).OrderBy(b => b.NetBalance).Select(b => (b.UserId, b.FullName, Amount: -b.NetBalance)).ToList();
        var suggestions = new List<SettleSuggestionDto>();
        var ci = 0, di = 0;
        while (ci < creditors.Count && di < debtors.Count)
        {
            var credit = creditors[ci];
            var debt = debtors[di];
            var amount = Math.Round(Math.Min(credit.Amount, debt.Amount), 2);
            if (amount > 0.01m) suggestions.Add(new SettleSuggestionDto(debt.UserId, debt.FullName, credit.UserId, credit.FullName, amount));
            creditors[ci] = (credit.UserId, credit.FullName, credit.Amount - amount);
            debtors[di] = (debt.UserId, debt.FullName, debt.Amount - amount);
            if (creditors[ci].Amount <= 0.01m) ci++;
            if (debtors[di].Amount <= 0.01m) di++;
        }
        return Ok(suggestions);
    }

    [HttpGet("settlements")]
    public async Task<ActionResult<IEnumerable<SettlementDto>>> Settlements(CancellationToken ct) =>
        Ok((await _db.Settlements.Include(x => x.FromUser).Include(x => x.ToUser).OrderByDescending(x => x.Date).ThenByDescending(x => x.CreatedAtUtc).ToListAsync(ct))
            .Select(x => new SettlementDto(x.Id, x.FromUserId, x.FromUser?.FullName ?? string.Empty, x.ToUserId, x.ToUser?.FullName ?? string.Empty, x.Amount, x.Date, x.Notes)));

    [HttpPost("settlements")]
    public async Task<ActionResult<SettlementDto>> RecordSettlement(RecordSettlementRequest request, CancellationToken ct)
    {
        var entity = new Settlement { FromUserId = request.FromUserId, ToUserId = request.ToUserId, Amount = request.Amount, Date = request.Date, Notes = request.Notes };
        _db.Settlements.Add(entity);
        await _db.SaveChangesAsync(ct);
        var from = await _db.Users.FindAsync(new object[] { request.FromUserId }, ct);
        var to = await _db.Users.FindAsync(new object[] { request.ToUserId }, ct);
        return Ok(new SettlementDto(entity.Id, entity.FromUserId, from?.FullName ?? string.Empty, entity.ToUserId, to?.FullName ?? string.Empty, entity.Amount, entity.Date, entity.Notes));
    }

    private static void ApplySplits(Expense entity, UpsertExpenseRequest request)
    {
        var participantIds = (request.SplitAmongUserIds != null && request.SplitAmongUserIds.Count > 0)
            ? request.SplitAmongUserIds.Distinct().ToList()
            : (request.PaidByUserId.HasValue ? new List<Guid> { request.PaidByUserId.Value } : new List<Guid>());
        if (participantIds.Count == 0) return;
        var share = Math.Round(request.Amount / participantIds.Count, 2);
        var allocated = share * (participantIds.Count - 1);
        for (var i = 0; i < participantIds.Count; i++)
        {
            var amount = i == participantIds.Count - 1 ? request.Amount - allocated : share;
            entity.Splits.Add(new ExpenseSplit { UserId = participantIds[i], ShareAmount = amount });
        }
    }

    private async Task ReloadNavigationsAsync(Expense entity, CancellationToken ct)
    {
        await _db.Entry(entity).Reference(x => x.PaidByUser).LoadAsync(ct);
        await _db.Entry(entity).Collection(x => x.Splits).Query().Include(s => s.User).LoadAsync(ct);
    }

    private async Task<List<ExpenseBalanceDto>> ComputeBalancesAsync(CancellationToken ct)
    {
        var users = await _db.Users.Where(u => u.IsActive).ToListAsync(ct);
        var expenses = await _db.Expenses.Include(x => x.Splits).ToListAsync(ct);
        var settlements = await _db.Settlements.ToListAsync(ct);
        return users.Select(u =>
        {
            var paid = expenses.Where(e => e.PaidByUserId == u.Id).Sum(e => e.Amount);
            var share = expenses.SelectMany(e => e.Splits).Where(s => s.UserId == u.Id).Sum(s => s.ShareAmount);
            var settleOut = settlements.Where(s => s.FromUserId == u.Id).Sum(s => s.Amount);
            var settleIn = settlements.Where(s => s.ToUserId == u.Id).Sum(s => s.Amount);
            return new ExpenseBalanceDto(u.Id, u.FullName, paid, share, paid - share - settleOut + settleIn);
        }).ToList();
    }
}
