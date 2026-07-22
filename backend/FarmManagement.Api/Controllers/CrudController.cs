using FarmManagement.Api.Domain;
using FarmManagement.Api.Dtos;
using FarmManagement.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public abstract class CrudController<TEntity, TDto, TCreate> : ControllerBase where TEntity : BaseEntity, new()
{
    protected readonly IRepository<TEntity> Repository;
    private readonly Func<TEntity, TDto> _toDto;
    private readonly Func<TCreate, TEntity> _createEntity;
    private readonly Action<TCreate, TEntity> _updateEntity;

    protected CrudController(IRepository<TEntity> repository, Func<TEntity, TDto> toDto, Func<TCreate, TEntity> createEntity, Action<TCreate, TEntity> updateEntity)
    {
        Repository = repository;
        _toDto = toDto;
        _createEntity = createEntity;
        _updateEntity = updateEntity;
    }

    [HttpGet]
    public virtual async Task<ActionResult<PagedResult<TDto>>> Get([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var query = ApplySearch(Repository.Query(), search);
        var total = await query.CountAsync(ct);
        var entities = await query.OrderByDescending(x => x.CreatedAtUtc).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return Ok(new PagedResult<TDto>(entities.Select(_toDto).ToList(), total, page, pageSize));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TDto>> GetById(Guid id, CancellationToken ct)
    {
        var entity = await Repository.GetByIdAsync(id, ct);
        return entity is null ? NotFound() : Ok(_toDto(entity));
    }

    [HttpPost]
    public virtual async Task<ActionResult<TDto>> Create(TCreate request, CancellationToken ct)
    {
        var entity = _createEntity(request);
        await Repository.AddAsync(entity, ct);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, _toDto(entity));
    }

    [HttpPut("{id:guid}")]
    public virtual async Task<ActionResult<TDto>> Update(Guid id, TCreate request, CancellationToken ct)
    {
        var entity = await Repository.GetByIdAsync(id, ct);
        if (entity is null) return NotFound();
        _updateEntity(request, entity);
        entity.Id = id;
        await Repository.UpdateAsync(entity, ct);
        return Ok(_toDto(entity));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var entity = await Repository.GetByIdAsync(id, ct);
        if (entity is null) return NotFound();
        await Repository.SoftDeleteAsync(entity, ct);
        return NoContent();
    }

    protected virtual IQueryable<TEntity> ApplySearch(IQueryable<TEntity> query, string? search) => query;
}
