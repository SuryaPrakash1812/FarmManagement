# Database Schema

All main entities inherit auditable soft-delete columns: `Id`, `CreatedAtUtc`, `UpdatedAtUtc`, `IsDeleted`.

Core tables:

- `Users`: authentication, profile, role, active state
- `Animals`: tag, species, breed, gender, birth/purchase details, value, weight, health, pregnancy, parents, photo, status
- `StockItems`, `StockMovements`: inventory and stock history
- `Customers`, `Sales`: customer sales, GST, discount, payment status, invoice
- `Vendors`, `Purchases`: procurement and vendor references
- `Investments`: construction, machinery, land, animal purchase, equipment
- `Expenses`: feed, medicine, salary, utilities, fuel, transport, miscellaneous
- `Payments`: incoming/outgoing, pending/paid, method, reference
- `Incomes`: milk, animal sales, eggs, subsidies, other income
- `HealthRecords`: vaccination, doctor visit, treatment, deworming, pregnancy reminders
- `BreedingRecords`: mating, expected delivery, delivery, newborn details
- `Employees`, `AttendanceRecords`: staff, salary, task and attendance tracking
- `FarmSettings`: farm identity, logo, currency, email, notifications
- `ActivityLogs`, `AuditLogs`: operational history

Foreign keys:

- `Animals.FatherId -> Animals.Id`
- `Animals.MotherId -> Animals.Id`
- `HealthRecords.AnimalId -> Animals.Id`
- `BreedingRecords.MaleAnimalId -> Animals.Id`
- `BreedingRecords.FemaleAnimalId -> Animals.Id`
- `StockMovements.StockItemId -> StockItems.Id`
- `Sales.CustomerId -> Customers.Id`
- `Purchases.VendorId -> Vendors.Id`
- `AttendanceRecords.EmployeeId -> Employees.Id`
