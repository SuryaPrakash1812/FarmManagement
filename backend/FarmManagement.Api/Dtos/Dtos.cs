using FarmManagement.Api.Domain;

namespace FarmManagement.Api.Dtos;

public sealed record LoginRequest(string Email, string Password);
public sealed record AuthResponse(string Token, DateTime ExpiresAtUtc, UserDto User);
public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);
public sealed record ForgotPasswordRequest(string Email);
public sealed record UserDto(Guid Id, string FullName, string Email, UserRole Role, string? Phone, string? AvatarUrl);
public sealed record CreateUserRequest(string FullName, string Email, string Password, UserRole Role, string? Phone);

public sealed record AnimalDto(Guid Id, string AnimalCode, string TagNumber, string Name, string Species, string Breed, Gender Gender, DateOnly? DateOfBirth, DateOnly? PurchaseDate, decimal PurchasePrice, decimal CurrentValue, decimal? Weight, string HealthStatus, string? VaccinationDetails, string? MedicalHistory, bool IsPregnant, Guid? FatherId, Guid? MotherId, string? PhotoUrl, string? Notes, AnimalStatus Status);
public sealed record UpsertAnimalRequest(string AnimalCode, string TagNumber, string Name, string Species, string Breed, Gender Gender, DateOnly? DateOfBirth, DateOnly? PurchaseDate, decimal PurchasePrice, decimal CurrentValue, decimal? Weight, string HealthStatus, string? VaccinationDetails, string? MedicalHistory, bool IsPregnant, Guid? FatherId, Guid? MotherId, string? Notes, AnimalStatus Status);

public sealed record StockItemDto(Guid Id, string ItemName, string Category, decimal Quantity, string Unit, decimal Cost, string? Supplier, DateOnly PurchaseDate, DateOnly? ExpiryDate, decimal ReorderLevel, string? Barcode, bool IsLowStock);
public sealed record UpsertStockItemRequest(string ItemName, string Category, decimal Quantity, string Unit, decimal Cost, string? Supplier, DateOnly PurchaseDate, DateOnly? ExpiryDate, decimal ReorderLevel, string? Barcode);
public sealed record SaleDto(Guid Id, string? CustomerName, string ProductType, string ProductName, decimal Quantity, decimal Amount, decimal Gst, decimal Discount, PaymentStatus PaymentStatus, string InvoiceNumber, DateOnly Date);
public sealed record UpsertSaleRequest(Guid? CustomerId, string ProductType, string ProductName, decimal Quantity, decimal Amount, decimal Gst, decimal Discount, PaymentStatus PaymentStatus, string InvoiceNumber, DateOnly Date);
public sealed record PurchaseDto(Guid Id, string? VendorName, string ItemName, decimal Quantity, decimal Cost, string PaymentMethod, string? InvoiceUrl, DateOnly PurchaseDate);
public sealed record UpsertPurchaseRequest(Guid? VendorId, string ItemName, decimal Quantity, decimal Cost, string PaymentMethod, DateOnly PurchaseDate);
public sealed record MoneyRecordDto(Guid Id, string CategoryOrSource, decimal Amount, string? PaymentMethod, DateOnly Date, string? Notes);
public sealed record ExpenseSplitDto(Guid UserId, string UserName, decimal ShareAmount);
public sealed record ExpenseDto(Guid Id, string Category, decimal Amount, string PaymentMethod, DateOnly Date, string? Notes, Guid? PaidByUserId, string? PaidByName, bool IsRecurring, string? RecurrenceInterval, IReadOnlyList<ExpenseSplitDto> Splits);
public sealed record UpsertExpenseRequest(string Category, decimal Amount, string PaymentMethod, DateOnly Date, string? Notes, Guid? PaidByUserId, bool IsRecurring, string? RecurrenceInterval, List<Guid>? SplitAmongUserIds);
public sealed record ExpenseBalanceDto(Guid UserId, string FullName, decimal Paid, decimal Share, decimal NetBalance);
public sealed record SettleSuggestionDto(Guid FromUserId, string FromName, Guid ToUserId, string ToName, decimal Amount);
public sealed record SettlementDto(Guid Id, Guid FromUserId, string FromName, Guid ToUserId, string ToName, decimal Amount, DateOnly Date, string? Notes);
public sealed record RecordSettlementRequest(Guid FromUserId, Guid ToUserId, decimal Amount, DateOnly Date, string? Notes);
public sealed record UpsertIncomeRequest(string Source, decimal Amount, DateOnly Date, string? Notes);
public sealed record InvestmentDto(Guid Id, string InvestmentType, decimal Amount, DateOnly Date, string? Description);
public sealed record UpsertInvestmentRequest(string InvestmentType, decimal Amount, DateOnly Date, string? Description);
public sealed record PaymentDto(Guid Id, PaymentDirection Direction, decimal Amount, PaymentStatus Status, string Method, string? TransactionReference, DateOnly DueDate, DateOnly? PaidDate, string? PartyName);
public sealed record UpsertPaymentRequest(PaymentDirection Direction, decimal Amount, PaymentStatus Status, string Method, string? TransactionReference, DateOnly DueDate, DateOnly? PaidDate, string? PartyName);
public sealed record HealthRecordDto(Guid Id, Guid AnimalId, string AnimalName, string RecordType, DateOnly Date, DateOnly? NextDueDate, string? DoctorName, string? Medicines, string? Diagnosis, string? Notes);
public sealed record UpsertHealthRecordRequest(Guid AnimalId, string RecordType, DateOnly Date, DateOnly? NextDueDate, string? DoctorName, string? Medicines, string? Diagnosis, string? Notes);
public sealed record BreedingRecordDto(Guid Id, Guid MaleAnimalId, Guid FemaleAnimalId, DateOnly MatingDate, DateOnly? ExpectedDeliveryDate, DateOnly? DeliveryDate, string? NewbornDetails);
public sealed record UpsertBreedingRecordRequest(Guid MaleAnimalId, Guid FemaleAnimalId, DateOnly MatingDate, DateOnly? ExpectedDeliveryDate, DateOnly? DeliveryDate, string? NewbornDetails);
public sealed record EmployeeDto(Guid Id, string FullName, string Role, decimal Salary, string? Phone, string? Address, string? Tasks);
public sealed record UpsertEmployeeRequest(string FullName, string Role, decimal Salary, string? Phone, string? Address, string? Tasks);
public sealed record AttendanceDto(Guid Id, Guid EmployeeId, string EmployeeName, DateOnly Date, bool IsPresent, string? Notes);
public sealed record UpsertAttendanceRequest(Guid EmployeeId, DateOnly Date, bool IsPresent, string? Notes);
public sealed record FarmSettingDto(Guid Id, string FarmName, string Currency, string? LogoUrl, string? EmailFrom, bool EnableNotifications);
public sealed record DashboardDto(decimal TotalAnimals, decimal TotalStock, decimal MonthlyIncome, decimal MonthlyExpenses, decimal PendingPayments, decimal TodaysSales, IEnumerable<ActivityDto> RecentActivities, IEnumerable<ChartPointDto> IncomeVsExpenses, IEnumerable<ChartPointDto> AnimalCountByCategory, decimal TotalInvestment, decimal Roi);
public sealed record ActivityDto(DateTime CreatedAtUtc, string Action, string EntityName, string? Details);
public sealed record ChartPointDto(string Label, decimal Value, decimal? SecondaryValue = null);
public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);
