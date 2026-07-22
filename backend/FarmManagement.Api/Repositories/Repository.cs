using FarmManagement.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Api.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    IQueryable<T> Query();
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<T> AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task SoftDeleteAsync(T entity, CancellationToken ct = default);
}

public sealed class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly Data.FarmDbContext _db;
    public Repository(Data.FarmDbContext db) => _db = db;
    public IQueryable<T> Query() => _db.Set<T>().AsNoTracking();
    public Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default) => _db.Set<T>().FirstOrDefaultAsync(x => x.Id == id, ct);
    public async Task<T> AddAsync(T entity, CancellationToken ct = default) { _db.Set<T>().Add(entity); await _db.SaveChangesAsync(ct); return entity; }
    public async Task UpdateAsync(T entity, CancellationToken ct = default) { _db.Set<T>().Update(entity); await _db.SaveChangesAsync(ct); }
    public async Task SoftDeleteAsync(T entity, CancellationToken ct = default) { entity.IsDeleted = true; _db.Set<T>().Update(entity); await _db.SaveChangesAsync(ct); }
}
