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

        AppUser? admin = await db.Users.FirstOrDefaultAsync(u => u.Email == "admin@farm.local", ct);
        if (admin is null)
        {
            admin = new AppUser { FullName = "Farm Admin", Email = "admin@farm.local", Role = UserRole.Admin };
            admin.PasswordHash = hasher.HashPassword(admin, "Admin@12345");
            db.Users.Add(admin);
        }

        if (!await db.FarmSettings.AnyAsync(ct)) db.FarmSettings.Add(new FarmSetting());

        await db.SaveChangesAsync(ct);
    }
}

