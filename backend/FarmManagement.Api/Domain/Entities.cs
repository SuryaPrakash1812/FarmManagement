namespace FarmManagement.Api.Domain;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
    public bool IsDeleted { get; set; }
}

public enum UserRole { Admin, Manager, Worker }
public enum AnimalStatus { Active, Sold, Dead }
public enum Gender { Female, Male, Unknown }
public enum PaymentStatus { Pending, PartiallyPaid, Paid, Cancelled }
public enum PaymentDirection { Incoming, Outgoing }

public sealed class AppUser : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Worker;
    public string? Phone { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; } = true;
}

public sealed class Animal : BaseEntity
{
    public string AnimalCode { get; set; } = string.Empty;
    public string TagNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public string Breed { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public DateOnly? PurchaseDate { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal? Weight { get; set; }
    public string HealthStatus { get; set; } = "Healthy";
    public string? VaccinationDetails { get; set; }
    public string? MedicalHistory { get; set; }
    public bool IsPregnant { get; set; }
    public Guid? FatherId { get; set; }
    public Animal? Father { get; set; }
    public Guid? MotherId { get; set; }
    public Animal? Mother { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Notes { get; set; }
    public AnimalStatus Status { get; set; } = AnimalStatus.Active;
    public ICollection<HealthRecord> HealthRecords { get; set; } = new List<HealthRecord>();
}

public sealed class StockItem : BaseEntity
{
    public string ItemName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public string? Supplier { get; set; }
    public DateOnly PurchaseDate { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public decimal ReorderLevel { get; set; }
    public string? Barcode { get; set; }
    public ICollection<StockMovement> Movements { get; set; } = new List<StockMovement>();
}

public sealed class StockMovement : BaseEntity
{
    public Guid StockItemId { get; set; }
    public StockItem? StockItem { get; set; }
    public string MovementType { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? Reference { get; set; }
}

public sealed class Customer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
}

public sealed class Sale : BaseEntity
{
    public Guid? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public string ProductType { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal Amount { get; set; }
    public decimal Gst { get; set; }
    public decimal Discount { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
}

public sealed class Vendor : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
}

public sealed class Purchase : BaseEntity
{
    public Guid? VendorId { get; set; }
    public Vendor? Vendor { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal Cost { get; set; }
    public string PaymentMethod { get; set; } = "Cash";
    public string? InvoiceUrl { get; set; }
    public DateOnly PurchaseDate { get; set; }
}

public sealed class Investment : BaseEntity
{
    public string InvestmentType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateOnly Date { get; set; }
    public string? Description { get; set; }
}

public sealed class Expense : BaseEntity
{
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = "Cash";
    public DateOnly Date { get; set; }
    public string? Notes { get; set; }
    public Guid? PaidByUserId { get; set; }
    public AppUser? PaidByUser { get; set; }
    public bool IsRecurring { get; set; }
    public string? RecurrenceInterval { get; set; }
    public ICollection<ExpenseSplit> Splits { get; set; } = new List<ExpenseSplit>();
}

public sealed class ExpenseSplit : BaseEntity
{
    public Guid ExpenseId { get; set; }
    public Expense? Expense { get; set; }
    public Guid UserId { get; set; }
    public AppUser? User { get; set; }
    public decimal ShareAmount { get; set; }
}

public sealed class Settlement : BaseEntity
{
    public Guid FromUserId { get; set; }
    public AppUser? FromUser { get; set; }
    public Guid ToUserId { get; set; }
    public AppUser? ToUser { get; set; }
    public decimal Amount { get; set; }
    public DateOnly Date { get; set; }
    public string? Notes { get; set; }
}

public sealed class Payment : BaseEntity
{
    public PaymentDirection Direction { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public string Method { get; set; } = "Cash";
    public string? TransactionReference { get; set; }
    public DateOnly DueDate { get; set; }
    public DateOnly? PaidDate { get; set; }
    public string? PartyName { get; set; }
}

public sealed class Income : BaseEntity
{
    public string Source { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateOnly Date { get; set; }
    public string? Notes { get; set; }
}

public sealed class HealthRecord : BaseEntity
{
    public Guid AnimalId { get; set; }
    public Animal? Animal { get; set; }
    public string RecordType { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public DateOnly? NextDueDate { get; set; }
    public string? DoctorName { get; set; }
    public string? Medicines { get; set; }
    public string? Diagnosis { get; set; }
    public string? Notes { get; set; }
}

public sealed class BreedingRecord : BaseEntity
{
    public Guid MaleAnimalId { get; set; }
    public Animal? MaleAnimal { get; set; }
    public Guid FemaleAnimalId { get; set; }
    public Animal? FemaleAnimal { get; set; }
    public DateOnly MatingDate { get; set; }
    public DateOnly? ExpectedDeliveryDate { get; set; }
    public DateOnly? DeliveryDate { get; set; }
    public string? NewbornDetails { get; set; }
}

public sealed class Employee : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Tasks { get; set; }
    public ICollection<AttendanceRecord> Attendance { get; set; } = new List<AttendanceRecord>();
}

public sealed class AttendanceRecord : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public DateOnly Date { get; set; }
    public bool IsPresent { get; set; }
    public string? Notes { get; set; }
}

public sealed class FarmSetting : BaseEntity
{
    public string FarmName { get; set; } = "Green Valley Farm";
    public string Currency { get; set; } = "INR";
    public string? LogoUrl { get; set; }
    public string? EmailFrom { get; set; }
    public bool EnableNotifications { get; set; } = true;
}

public sealed class ActivityLog : BaseEntity
{
    public string Actor { get; set; } = "System";
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public string? Details { get; set; }
}

public sealed class AuditLog : BaseEntity
{
    public string EntityName { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? ChangedBy { get; set; }
    public string? Changes { get; set; }
}
