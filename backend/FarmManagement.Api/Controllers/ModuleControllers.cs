using FarmManagement.Api.Data;
using FarmManagement.Api.Domain;
using FarmManagement.Api.Dtos;
using FarmManagement.Api.Mapping;
using FarmManagement.Api.Repositories;
using FarmManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Api.Controllers;

public sealed class AnimalsController : CrudController<Animal, AnimalDto, UpsertAnimalRequest>
{
    private readonly IFileStorageService _files;
    private readonly FarmDbContext _db;
    public AnimalsController(IRepository<Animal> repository, IFileStorageService files, FarmDbContext db) : base(repository, EntityMapper.ToDto, EntityMapper.ToEntity, EntityMapper.Apply) { _files = files; _db = db; }
    protected override IQueryable<Animal> ApplySearch(IQueryable<Animal> query, string? search) => string.IsNullOrWhiteSpace(search) ? query : query.Where(x => x.Name.Contains(search) || x.TagNumber.Contains(search) || x.Species.Contains(search) || x.Breed.Contains(search));

    [HttpPost("{id:guid}/photo")]
    [RequestSizeLimit(8_000_000)]
    public async Task<ActionResult<AnimalDto>> UploadPhoto(Guid id, IFormFile photo, CancellationToken ct)
    {
        var animal = await _db.Animals.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (animal is null) return NotFound();
        animal.PhotoUrl = await _files.SaveAsync(photo, "animals", ct);
        await _db.SaveChangesAsync(ct);
        return Ok(EntityMapper.ToDto(animal));
    }
}

public sealed class StockController : CrudController<StockItem, StockItemDto, UpsertStockItemRequest>
{
    public StockController(IRepository<StockItem> repository) : base(repository, EntityMapper.ToDto, EntityMapper.ToEntity, EntityMapper.Apply) { }
    protected override IQueryable<StockItem> ApplySearch(IQueryable<StockItem> query, string? search) => string.IsNullOrWhiteSpace(search) ? query : query.Where(x => x.ItemName.Contains(search) || x.Category.Contains(search) || (x.Supplier != null && x.Supplier.Contains(search)));
    [HttpGet("low-stock")] public async Task<ActionResult<IEnumerable<StockItemDto>>> LowStock(CancellationToken ct) => Ok((await Repository.Query().Where(x => x.Quantity <= x.ReorderLevel).ToListAsync(ct)).Select(EntityMapper.ToDto));
}

public sealed class SalesController : CrudController<Sale, SaleDto, UpsertSaleRequest>
{
    public SalesController(IRepository<Sale> repository) : base(repository, EntityMapper.ToDto, EntityMapper.ToEntity, EntityMapper.Apply) { }
    protected override IQueryable<Sale> ApplySearch(IQueryable<Sale> query, string? search) => string.IsNullOrWhiteSpace(search) ? query.Include(x => x.Customer) : query.Include(x => x.Customer).Where(x => x.ProductName.Contains(search) || x.InvoiceNumber.Contains(search));
    [HttpGet("{id:guid}/invoice")] public async Task<IActionResult> Invoice(Guid id, CancellationToken ct) { var sale = await Repository.Query().FirstOrDefaultAsync(x => x.Id == id, ct); return sale is null ? NotFound() : File(System.Text.Encoding.UTF8.GetBytes($"Invoice {sale.InvoiceNumber}\nAmount: {sale.Amount}\nGST: {sale.Gst}\nDiscount: {sale.Discount}"), "text/plain", $"{sale.InvoiceNumber}.txt"); }
}

public sealed class PurchasesController : CrudController<Purchase, PurchaseDto, UpsertPurchaseRequest> { public PurchasesController(IRepository<Purchase> r) : base(r, EntityMapper.ToDto, EntityMapper.ToEntity, EntityMapper.Apply) { } protected override IQueryable<Purchase> ApplySearch(IQueryable<Purchase> q, string? s) => string.IsNullOrWhiteSpace(s) ? q.Include(x => x.Vendor) : q.Include(x => x.Vendor).Where(x => x.ItemName.Contains(s) || x.PaymentMethod.Contains(s)); }
public sealed class InvestmentsController : CrudController<Investment, InvestmentDto, UpsertInvestmentRequest> { public InvestmentsController(IRepository<Investment> r) : base(r, EntityMapper.ToDto, EntityMapper.ToEntity, EntityMapper.Apply) { } protected override IQueryable<Investment> ApplySearch(IQueryable<Investment> q, string? s) => string.IsNullOrWhiteSpace(s) ? q : q.Where(x => x.InvestmentType.Contains(s)); }
public sealed class IncomesController : CrudController<Income, MoneyRecordDto, UpsertIncomeRequest> { public IncomesController(IRepository<Income> r) : base(r, EntityMapper.ToDto, EntityMapper.ToEntity, EntityMapper.Apply) { } protected override IQueryable<Income> ApplySearch(IQueryable<Income> q, string? s) => string.IsNullOrWhiteSpace(s) ? q : q.Where(x => x.Source.Contains(s)); }
public sealed class PaymentsController : CrudController<Payment, PaymentDto, UpsertPaymentRequest> { public PaymentsController(IRepository<Payment> r) : base(r, EntityMapper.ToDto, EntityMapper.ToEntity, EntityMapper.Apply) { } protected override IQueryable<Payment> ApplySearch(IQueryable<Payment> q, string? s) => string.IsNullOrWhiteSpace(s) ? q : q.Where(x => x.Method.Contains(s) || (x.PartyName != null && x.PartyName.Contains(s))); }
public sealed class HealthController : CrudController<HealthRecord, HealthRecordDto, UpsertHealthRecordRequest> { public HealthController(IRepository<HealthRecord> r) : base(r, EntityMapper.ToDto, EntityMapper.ToEntity, EntityMapper.Apply) { } protected override IQueryable<HealthRecord> ApplySearch(IQueryable<HealthRecord> q, string? s) => string.IsNullOrWhiteSpace(s) ? q.Include(x => x.Animal) : q.Include(x => x.Animal).Where(x => x.RecordType.Contains(s) || (x.Medicines != null && x.Medicines.Contains(s))); [HttpGet("reminders")] public async Task<ActionResult<IEnumerable<HealthRecordDto>>> Reminders(CancellationToken ct) => Ok((await Repository.Query().Include(x => x.Animal).Where(x => x.NextDueDate != null && x.NextDueDate <= DateOnly.FromDateTime(DateTime.Today.AddDays(14))).ToListAsync(ct)).Select(EntityMapper.ToDto)); }
public sealed class BreedingController : CrudController<BreedingRecord, BreedingRecordDto, UpsertBreedingRecordRequest> { public BreedingController(IRepository<BreedingRecord> r) : base(r, EntityMapper.ToDto, EntityMapper.ToEntity, EntityMapper.Apply) { } }
public sealed class EmployeesController : CrudController<Employee, EmployeeDto, UpsertEmployeeRequest> { public EmployeesController(IRepository<Employee> r) : base(r, EntityMapper.ToDto, EntityMapper.ToEntity, EntityMapper.Apply) { } protected override IQueryable<Employee> ApplySearch(IQueryable<Employee> q, string? s) => string.IsNullOrWhiteSpace(s) ? q : q.Where(x => x.FullName.Contains(s) || x.Role.Contains(s)); }
