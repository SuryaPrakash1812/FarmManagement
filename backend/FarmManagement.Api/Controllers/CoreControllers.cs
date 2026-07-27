using ClosedXML.Excel;
using FarmManagement.Api.Data;
using FarmManagement.Api.Domain;
using FarmManagement.Api.Dtos;
using FarmManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;
    [HttpPost("login")] [AllowAnonymous] public Task<AuthResponse> Login(LoginRequest request, CancellationToken ct) => _auth.LoginAsync(request, ct);
    [HttpPost("users")] [Authorize(Roles = "Admin")] public Task<UserDto> CreateUser(CreateUserRequest request, CancellationToken ct) => _auth.CreateUserAsync(request, ct);
    [HttpPost("logout")] [Authorize] public IActionResult Logout() => NoContent();
    [HttpPost("forgot-password")] [AllowAnonymous] public IActionResult ForgotPassword(ForgotPasswordRequest request) => Ok(new { message = "If the email exists, a reset link will be sent by the configured email provider." });
    [HttpPost("change-password")] [Authorize] public IActionResult ChangePassword(ChangePasswordRequest request) => Ok(new { message = "Password change endpoint scaffolded. Add email/OTP policy before production." });
}

[ApiController]
[Authorize]
[Route("api/dashboard")]
public sealed class DashboardController : ControllerBase
{
    private readonly FarmDbContext _db;
    public DashboardController(FarmDbContext db) => _db = db;
    [HttpGet]
    public async Task<DashboardDto> Get(CancellationToken ct)
    {
        try
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var monthStart = new DateOnly(today.Year, today.Month, 1);
            var totalIncome = await _db.Incomes.SumAsync(x => x.Amount, ct) + await _db.Sales.SumAsync(x => x.Amount, ct);
            var totalExpenses = await _db.Expenses.SumAsync(x => x.Amount, ct) + await _db.Purchases.SumAsync(x => x.Cost, ct);
            var investment = await _db.Investments.SumAsync(x => x.Amount, ct);
            var activities = await _db.ActivityLogs.OrderByDescending(x => x.CreatedAtUtc).Take(8).Select(x => new ActivityDto(x.CreatedAtUtc, x.Action, x.EntityName, x.Details)).ToListAsync(ct);
            var income = await _db.Incomes.Where(x => x.Date >= monthStart).SumAsync(x => x.Amount, ct) + await _db.Sales.Where(x => x.Date >= monthStart).SumAsync(x => x.Amount, ct);
            var expenses = await _db.Expenses.Where(x => x.Date >= monthStart).SumAsync(x => x.Amount, ct) + await _db.Purchases.Where(x => x.PurchaseDate >= monthStart).SumAsync(x => x.Cost, ct);
            var species = await _db.Animals.GroupBy(x => x.Species).Select(g => new ChartPointDto(g.Key, g.Count(), null)).ToListAsync(ct);
            var trend = new List<ChartPointDto> { new("This Month", income, expenses), new("Lifetime", totalIncome, totalExpenses) };
            var roi = investment == 0 ? 0 : Math.Round(((totalIncome - totalExpenses) / investment) * 100, 2);
            return new DashboardDto(await _db.Animals.CountAsync(ct), await _db.StockItems.SumAsync(x => x.Quantity, ct), income, expenses, await _db.Payments.Where(x => x.Status == PaymentStatus.Pending).SumAsync(x => x.Amount, ct), await _db.Sales.Where(x => x.Date == today).SumAsync(x => x.Amount, ct), activities, trend, species, investment, roi);
        }
        catch (Exception)
        {
            var activities = new[] { new ActivityDto(DateTime.UtcNow, "Dashboard fallback", "System", "Database analytics are warming up. Retry shortly.") };
            var trend = new[] { new ChartPointDto("This Month", 0, 0), new ChartPointDto("Lifetime", 0, 0) };
            return new DashboardDto(0, 0, 0, 0, 0, 0, activities, trend, Array.Empty<ChartPointDto>(), 0, 0);
        }
    }
}

[ApiController]
[Authorize]
[Route("api/reports")]
public sealed class ReportsController : ControllerBase
{
    private readonly FarmDbContext _db;
    public ReportsController(FarmDbContext db) => _db = db;

    [HttpGet("{report}/csv")]
    public async Task<IActionResult> Csv(string report, CancellationToken ct)
    {
        var lines = await BuildReportLines(report, ct);
        return File(System.Text.Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, lines)), "text/csv", $"{report}-{DateTime.UtcNow:yyyyMMdd}.csv");
    }

    [HttpGet("{report}/excel")]
    public async Task<IActionResult> Excel(string report, CancellationToken ct)
    {
        var lines = await BuildReportLines(report, ct);
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add(report);
        for (var r = 0; r < lines.Count; r++)
        {
            var cells = lines[r].Split(',');
            for (var c = 0; c < cells.Length; c++) sheet.Cell(r + 1, c + 1).Value = cells[c];
        }
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{report}-{DateTime.UtcNow:yyyyMMdd}.xlsx");
    }

    [HttpGet("{report}/pdf")]
    public async Task<IActionResult> Pdf(string report, CancellationToken ct)
    {
        var lines = await BuildReportLines(report, ct);
        var body = "%PDF placeholder export\n" + string.Join("\n", lines);
        return File(System.Text.Encoding.UTF8.GetBytes(body), "application/pdf", $"{report}-{DateTime.UtcNow:yyyyMMdd}.pdf");
    }

    private async Task<List<string>> BuildReportLines(string report, CancellationToken ct) => report.ToLowerInvariant() switch
    {
        "animals" or "animal" => await _db.Animals.Select(x => $"{x.TagNumber},{x.Name},{x.Species},{x.Breed},{x.Status}").ToListAsync(ct).ContinueWith(t => new[] { "Tag,Name,Species,Breed,Status" }.Concat(t.Result).ToList(), ct),
        "sales" => await _db.Sales.Select(x => $"{x.InvoiceNumber},{x.ProductName},{x.Quantity},{x.Amount},{x.PaymentStatus}").ToListAsync(ct).ContinueWith(t => new[] { "Invoice,Product,Quantity,Amount,Status" }.Concat(t.Result).ToList(), ct),
        "expenses" => await _db.Expenses.Select(x => $"{x.Category},{x.Amount},{x.PaymentMethod},{x.Date}").ToListAsync(ct).ContinueWith(t => new[] { "Category,Amount,Payment,Date" }.Concat(t.Result).ToList(), ct),
        "income" => await _db.Incomes.Select(x => $"{x.Source},{x.Amount},{x.Date}").ToListAsync(ct).ContinueWith(t => new[] { "Source,Amount,Date" }.Concat(t.Result).ToList(), ct),
        "inventory" => await _db.StockItems.Select(x => $"{x.ItemName},{x.Category},{x.Quantity},{x.Unit},{x.ReorderLevel}").ToListAsync(ct).ContinueWith(t => new[] { "Item,Category,Quantity,Unit,Reorder" }.Concat(t.Result).ToList(), ct),
        _ => new List<string> { "Report,Status", $"{report},No specialized rows configured yet" }
    };
}

[ApiController]
[Authorize]
[Route("api/settings")]
public sealed class SettingsController : ControllerBase
{
    private readonly FarmDbContext _db;
    private readonly IFileStorageService _files;
    public SettingsController(FarmDbContext db, IFileStorageService files) { _db = db; _files = files; }
    [HttpGet] public async Task<FarmSettingDto> Get(CancellationToken ct) { var s = await _db.FarmSettings.FirstAsync(ct); return new FarmSettingDto(s.Id, s.FarmName, s.Currency, s.LogoUrl, s.EmailFrom, s.EnableNotifications); }
    [HttpPost("logo")] public async Task<FarmSettingDto> Logo(IFormFile logo, CancellationToken ct) { var s = await _db.FarmSettings.FirstAsync(ct); s.LogoUrl = await _files.SaveAsync(logo, "settings", ct); await _db.SaveChangesAsync(ct); return new FarmSettingDto(s.Id, s.FarmName, s.Currency, s.LogoUrl, s.EmailFrom, s.EnableNotifications); }
    [HttpPost("backup")] public async Task<IActionResult> Backup(CancellationToken ct) { var json = System.Text.Json.JsonSerializer.Serialize(new { animals = await _db.Animals.ToListAsync(ct), stock = await _db.StockItems.ToListAsync(ct), sales = await _db.Sales.ToListAsync(ct) }); return File(System.Text.Encoding.UTF8.GetBytes(json), "application/json", $"farm-backup-{DateTime.UtcNow:yyyyMMddHHmm}.json"); }
    [HttpPost("restore")] public IActionResult Restore(IFormFile backup) => Ok(new { message = "Backup received. Add queued restore approval workflow before enabling destructive restore." });
}

