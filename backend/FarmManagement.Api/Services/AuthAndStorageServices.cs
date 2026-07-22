using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FarmManagement.Api.Data;
using FarmManagement.Api.Domain;
using FarmManagement.Api.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FarmManagement.Api.Services;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct);
    Task<UserDto> CreateUserAsync(CreateUserRequest request, CancellationToken ct);
}

public sealed class AuthService : IAuthService
{
    private readonly FarmDbContext _db;
    private readonly IConfiguration _config;
    private readonly PasswordHasher<AppUser> _hasher = new();
    public AuthService(FarmDbContext db, IConfiguration config) { _db = db; _config = config; }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == request.Email && x.IsActive, ct) ?? throw new UnauthorizedAccessException("Invalid credentials.");
        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed) throw new UnauthorizedAccessException("Invalid credentials.");
        var expires = DateTime.UtcNow.AddMinutes(_config.GetValue<int>("Jwt:ExpiresMinutes", 120));
        return new AuthResponse(CreateToken(user, expires), expires, new UserDto(user.Id, user.FullName, user.Email, user.Role, user.Phone, user.AvatarUrl));
    }

    public async Task<UserDto> CreateUserAsync(CreateUserRequest request, CancellationToken ct)
    {
        if (await _db.Users.AnyAsync(x => x.Email == request.Email, ct)) throw new InvalidOperationException("Email already exists.");
        var user = new AppUser { FullName = request.FullName, Email = request.Email, Role = request.Role, Phone = request.Phone };
        user.PasswordHash = _hasher.HashPassword(user, request.Password);
        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
        return new UserDto(user.Id, user.FullName, user.Email, user.Role, user.Phone, user.AvatarUrl);
    }

    private string CreateToken(AppUser user, DateTime expires)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!));
        var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), new Claim(ClaimTypes.Email, user.Email), new Claim(ClaimTypes.Name, user.FullName), new Claim(ClaimTypes.Role, user.Role.ToString()) };
        var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"], claims, expires: expires, signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public interface IFileStorageService
{
    Task<string> SaveAsync(IFormFile file, string folder, CancellationToken ct);
}

public sealed class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _config;
    public LocalFileStorageService(IWebHostEnvironment env, IConfiguration config) { _env = env; _config = config; }
    public async Task<string> SaveAsync(IFormFile file, string folder, CancellationToken ct)
    {
        if (file.Length == 0) throw new InvalidOperationException("Uploaded file is empty.");
        var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp", ".pdf" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowed.Contains(ext)) throw new InvalidOperationException("Unsupported file type.");
        var root = Path.Combine(_env.ContentRootPath, _config["Storage:UploadPath"] ?? "uploads", folder);
        Directory.CreateDirectory(root);
        var name = $"{Guid.NewGuid():N}{ext}";
        var path = Path.Combine(root, name);
        await using var stream = File.Create(path);
        await file.CopyToAsync(stream, ct);
        return $"/uploads/{folder}/{name}";
    }
}
