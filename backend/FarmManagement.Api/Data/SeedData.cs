using FarmManagement.Api.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Api.Data;

public static class SeedData
{
    public static async Task InitializeAsync(FarmDbContext db, CancellationToken ct = default)
    {
        await db.Database.MigrateAsync(ct);
        var hasher = new PasswordHasher<AppUser>();

        if (!await db.Users.AnyAsync(ct))
        {
            var admin = new AppUser { FullName = "Farm Admin", Email = "admin@farm.local", Role = UserRole.Admin };
            admin.PasswordHash = hasher.HashPassword(admin, "Admin@12345");
            db.Users.Add(admin);
        }

        if (!await db.Animals.AnyAsync(ct))
        {
            db.Animals.AddRange(
                new Animal { AnimalCode = "COW-001", TagNumber = "GVF-1001", Name = "Lakshmi", Species = "Cow", Breed = "Jersey", Gender = Gender.Female, DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-3)), PurchaseDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-2)), PurchasePrice = 55000, CurrentValue = 68000, Weight = 420, HealthStatus = "Healthy", VaccinationDetails = "FMD, HS", Status = AnimalStatus.Active },
                new Animal { AnimalCode = "GOAT-001", TagNumber = "GVF-2001", Name = "Meera", Species = "Goat", Breed = "Boer", Gender = Gender.Female, PurchaseDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(-8)), PurchasePrice = 9000, CurrentValue = 12000, Weight = 42, HealthStatus = "Observation", Status = AnimalStatus.Active }
            );
        }

        if (!await db.StockItems.AnyAsync(ct))
        {
            db.StockItems.AddRange(
                new StockItem { ItemName = "Cattle Feed", Category = "Feed", Quantity = 140, Unit = "kg", Cost = 4200, Supplier = "Agro Supplies", PurchaseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-12)), ReorderLevel = 50 },
                new StockItem { ItemName = "FMD Vaccine", Category = "Vaccines", Quantity = 8, Unit = "vial", Cost = 1600, Supplier = "Vet Care", PurchaseDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-20)), ExpiryDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(7)), ReorderLevel = 10 }
            );
        }

        if (!await db.Customers.AnyAsync(ct)) db.Customers.Add(new Customer { Name = "Daily Dairy Buyer", Phone = "+91-9000000000" });
        if (!await db.Sales.AnyAsync(ct)) db.Sales.Add(new Sale { ProductType = "Milk", ProductName = "Cow Milk", Quantity = 38, Amount = 2280, Gst = 0, Discount = 0, PaymentStatus = PaymentStatus.Paid, InvoiceNumber = "INV-0001", Date = DateOnly.FromDateTime(DateTime.Today) });
        if (!await db.Expenses.AnyAsync(ct)) db.Expenses.Add(new Expense { Category = "Feed", Amount = 4200, PaymentMethod = "UPI", Date = DateOnly.FromDateTime(DateTime.Today.AddDays(-12)), Notes = "Weekly feed purchase" });
        if (!await db.Incomes.AnyAsync(ct)) db.Incomes.Add(new Income { Source = "Milk", Amount = 2280, Date = DateOnly.FromDateTime(DateTime.Today) });
        if (!await db.Payments.AnyAsync(ct)) db.Payments.Add(new Payment { Direction = PaymentDirection.Incoming, Amount = 5500, Status = PaymentStatus.Pending, Method = "Bank Transfer", DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3)), PartyName = "Retail milk customer" });
        if (!await db.Investments.AnyAsync(ct)) db.Investments.Add(new Investment { InvestmentType = "Animal Purchase", Amount = 64000, Date = DateOnly.FromDateTime(DateTime.Today.AddYears(-1)), Description = "Initial livestock investment" });
        if (!await db.Employees.AnyAsync(ct)) db.Employees.Add(new Employee { FullName = "Ravi Kumar", Role = "Worker", Salary = 18000, Phone = "+91-9111111111", Tasks = "Milking, feeding, cleaning" });
        if (!await db.FarmSettings.AnyAsync(ct)) db.FarmSettings.Add(new FarmSetting());
        if (!await db.ActivityLogs.AnyAsync(ct)) db.ActivityLogs.Add(new ActivityLog { Action = "Seeded sample farm data", EntityName = "System", Details = "Initial dashboard data created" });

        await db.SaveChangesAsync(ct);
    }
}

