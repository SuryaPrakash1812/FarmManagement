using FarmManagement.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Api.Data;

public sealed class FarmDbContext : DbContext
{
    public FarmDbContext(DbContextOptions<FarmDbContext> options) : base(options) { }

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Animal> Animals => Set<Animal>();
    public DbSet<StockItem> StockItems => Set<StockItem>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<Purchase> Purchases => Set<Purchase>();
    public DbSet<Investment> Investments => Set<Investment>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<ExpenseSplit> ExpenseSplits => Set<ExpenseSplit>();
    public DbSet<Settlement> Settlements => Set<Settlement>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Income> Incomes => Set<Income>();
    public DbSet<HealthRecord> HealthRecords => Set<HealthRecord>();
    public DbSet<BreedingRecord> BreedingRecords => Set<BreedingRecord>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();
    public DbSet<FarmSetting> FarmSettings => Set<FarmSetting>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<Animal>().HasIndex(x => x.TagNumber).IsUnique();
        modelBuilder.Entity<Animal>().HasOne(x => x.Father).WithMany().HasForeignKey(x => x.FatherId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Animal>().HasOne(x => x.Mother).WithMany().HasForeignKey(x => x.MotherId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<BreedingRecord>().HasOne(x => x.MaleAnimal).WithMany().HasForeignKey(x => x.MaleAnimalId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<BreedingRecord>().HasOne(x => x.FemaleAnimal).WithMany().HasForeignKey(x => x.FemaleAnimalId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Expense>().HasOne(x => x.PaidByUser).WithMany().HasForeignKey(x => x.PaidByUserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ExpenseSplit>().HasOne(x => x.Expense).WithMany(x => x.Splits).HasForeignKey(x => x.ExpenseId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<ExpenseSplit>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Settlement>().HasOne(x => x.FromUser).WithMany().HasForeignKey(x => x.FromUserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Settlement>().HasOne(x => x.ToUser).WithMany().HasForeignKey(x => x.ToUserId).OnDelete(DeleteBehavior.Restrict);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes().Where(t => typeof(BaseEntity).IsAssignableFrom(t.ClrType)))
        {
            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(BuildSoftDeleteFilter(entityType.ClrType));
        }

        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added) entry.Entity.CreatedAtUtc = DateTime.UtcNow;
            if (entry.State == EntityState.Modified) entry.Entity.UpdatedAtUtc = DateTime.UtcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    private static System.Linq.Expressions.LambdaExpression BuildSoftDeleteFilter(Type entityType)
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(entityType, "e");
        var property = System.Linq.Expressions.Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
        var compare = System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(false));
        return System.Linq.Expressions.Expression.Lambda(compare, parameter);
    }
}
