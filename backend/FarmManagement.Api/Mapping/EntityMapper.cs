using FarmManagement.Api.Domain;
using FarmManagement.Api.Dtos;

namespace FarmManagement.Api.Mapping;

public static class EntityMapper
{
    public static UserDto ToDto(AppUser x) => new(x.Id, x.FullName, x.Email, x.Role, x.Phone, x.AvatarUrl);
    public static AnimalDto ToDto(Animal x) => new(x.Id, x.AnimalCode, x.TagNumber, x.Name, x.Species, x.Breed, x.Gender, x.DateOfBirth, x.PurchaseDate, x.PurchasePrice, x.CurrentValue, x.Weight, x.HealthStatus, x.VaccinationDetails, x.MedicalHistory, x.IsPregnant, x.FatherId, x.MotherId, x.PhotoUrl, x.Notes, x.Status);
    public static Animal ToEntity(UpsertAnimalRequest x) { var e = new Animal(); Apply(x, e); return e; }
    public static void Apply(UpsertAnimalRequest x, Animal e) { e.AnimalCode = x.AnimalCode; e.TagNumber = x.TagNumber; e.Name = x.Name; e.Species = x.Species; e.Breed = x.Breed; e.Gender = x.Gender; e.DateOfBirth = x.DateOfBirth; e.PurchaseDate = x.PurchaseDate; e.PurchasePrice = x.PurchasePrice; e.CurrentValue = x.CurrentValue; e.Weight = x.Weight; e.HealthStatus = x.HealthStatus; e.VaccinationDetails = x.VaccinationDetails; e.MedicalHistory = x.MedicalHistory; e.IsPregnant = x.IsPregnant; e.FatherId = x.FatherId; e.MotherId = x.MotherId; e.Notes = x.Notes; e.Status = x.Status; }

    public static StockItemDto ToDto(StockItem x) => new(x.Id, x.ItemName, x.Category, x.Quantity, x.Unit, x.Cost, x.Supplier, x.PurchaseDate, x.ExpiryDate, x.ReorderLevel, x.Barcode, x.Quantity <= x.ReorderLevel);
    public static StockItem ToEntity(UpsertStockItemRequest x) { var e = new StockItem(); Apply(x, e); return e; }
    public static void Apply(UpsertStockItemRequest x, StockItem e) { e.ItemName = x.ItemName; e.Category = x.Category; e.Quantity = x.Quantity; e.Unit = x.Unit; e.Cost = x.Cost; e.Supplier = x.Supplier; e.PurchaseDate = x.PurchaseDate; e.ExpiryDate = x.ExpiryDate; e.ReorderLevel = x.ReorderLevel; e.Barcode = x.Barcode; }

    public static SaleDto ToDto(Sale x) => new(x.Id, x.Customer?.Name, x.ProductType, x.ProductName, x.Quantity, x.Amount, x.Gst, x.Discount, x.PaymentStatus, x.InvoiceNumber, x.Date);
    public static Sale ToEntity(UpsertSaleRequest x) { var e = new Sale(); Apply(x, e); return e; }
    public static void Apply(UpsertSaleRequest x, Sale e) { e.CustomerId = x.CustomerId; e.ProductType = x.ProductType; e.ProductName = x.ProductName; e.Quantity = x.Quantity; e.Amount = x.Amount; e.Gst = x.Gst; e.Discount = x.Discount; e.PaymentStatus = x.PaymentStatus; e.InvoiceNumber = x.InvoiceNumber; e.Date = x.Date; }

    public static PurchaseDto ToDto(Purchase x) => new(x.Id, x.Vendor?.Name, x.ItemName, x.Quantity, x.Cost, x.PaymentMethod, x.InvoiceUrl, x.PurchaseDate);
    public static Purchase ToEntity(UpsertPurchaseRequest x) { var e = new Purchase(); Apply(x, e); return e; }
    public static void Apply(UpsertPurchaseRequest x, Purchase e) { e.VendorId = x.VendorId; e.ItemName = x.ItemName; e.Quantity = x.Quantity; e.Cost = x.Cost; e.PaymentMethod = x.PaymentMethod; e.PurchaseDate = x.PurchaseDate; }

    public static InvestmentDto ToDto(Investment x) => new(x.Id, x.InvestmentType, x.Amount, x.Date, x.Description);
    public static Investment ToEntity(UpsertInvestmentRequest x) { var e = new Investment(); Apply(x, e); return e; }
    public static void Apply(UpsertInvestmentRequest x, Investment e) { e.InvestmentType = x.InvestmentType; e.Amount = x.Amount; e.Date = x.Date; e.Description = x.Description; }

    public static MoneyRecordDto ToDto(Expense x) => new(x.Id, x.Category, x.Amount, x.PaymentMethod, x.Date, x.Notes);
    public static Expense ToEntity(UpsertExpenseRequest x) { var e = new Expense(); Apply(x, e); return e; }
    public static void Apply(UpsertExpenseRequest x, Expense e) { e.Category = x.Category; e.Amount = x.Amount; e.PaymentMethod = x.PaymentMethod; e.Date = x.Date; e.Notes = x.Notes; }

    public static MoneyRecordDto ToDto(Income x) => new(x.Id, x.Source, x.Amount, null, x.Date, x.Notes);
    public static Income ToEntity(UpsertIncomeRequest x) { var e = new Income(); Apply(x, e); return e; }
    public static void Apply(UpsertIncomeRequest x, Income e) { e.Source = x.Source; e.Amount = x.Amount; e.Date = x.Date; e.Notes = x.Notes; }

    public static PaymentDto ToDto(Payment x) => new(x.Id, x.Direction, x.Amount, x.Status, x.Method, x.TransactionReference, x.DueDate, x.PaidDate, x.PartyName);
    public static Payment ToEntity(UpsertPaymentRequest x) { var e = new Payment(); Apply(x, e); return e; }
    public static void Apply(UpsertPaymentRequest x, Payment e) { e.Direction = x.Direction; e.Amount = x.Amount; e.Status = x.Status; e.Method = x.Method; e.TransactionReference = x.TransactionReference; e.DueDate = x.DueDate; e.PaidDate = x.PaidDate; e.PartyName = x.PartyName; }

    public static HealthRecordDto ToDto(HealthRecord x) => new(x.Id, x.AnimalId, x.Animal?.Name ?? string.Empty, x.RecordType, x.Date, x.NextDueDate, x.DoctorName, x.Medicines, x.Diagnosis, x.Notes);
    public static HealthRecord ToEntity(UpsertHealthRecordRequest x) { var e = new HealthRecord(); Apply(x, e); return e; }
    public static void Apply(UpsertHealthRecordRequest x, HealthRecord e) { e.AnimalId = x.AnimalId; e.RecordType = x.RecordType; e.Date = x.Date; e.NextDueDate = x.NextDueDate; e.DoctorName = x.DoctorName; e.Medicines = x.Medicines; e.Diagnosis = x.Diagnosis; e.Notes = x.Notes; }

    public static BreedingRecordDto ToDto(BreedingRecord x) => new(x.Id, x.MaleAnimalId, x.FemaleAnimalId, x.MatingDate, x.ExpectedDeliveryDate, x.DeliveryDate, x.NewbornDetails);
    public static BreedingRecord ToEntity(UpsertBreedingRecordRequest x) { var e = new BreedingRecord(); Apply(x, e); return e; }
    public static void Apply(UpsertBreedingRecordRequest x, BreedingRecord e) { e.MaleAnimalId = x.MaleAnimalId; e.FemaleAnimalId = x.FemaleAnimalId; e.MatingDate = x.MatingDate; e.ExpectedDeliveryDate = x.ExpectedDeliveryDate; e.DeliveryDate = x.DeliveryDate; e.NewbornDetails = x.NewbornDetails; }

    public static EmployeeDto ToDto(Employee x) => new(x.Id, x.FullName, x.Role, x.Salary, x.Phone, x.Address, x.Tasks);
    public static Employee ToEntity(UpsertEmployeeRequest x) { var e = new Employee(); Apply(x, e); return e; }
    public static void Apply(UpsertEmployeeRequest x, Employee e) { e.FullName = x.FullName; e.Role = x.Role; e.Salary = x.Salary; e.Phone = x.Phone; e.Address = x.Address; e.Tasks = x.Tasks; }
}
