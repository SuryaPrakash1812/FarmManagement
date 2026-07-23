using FarmManagement.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Api.Services;

public sealed class DatabaseInitializerHostedService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<DatabaseInitializerHostedService> _logger;

    public DatabaseInitializerHostedService(IServiceProvider services, ILogger<DatabaseInitializerHostedService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        for (var attempt = 1; attempt <= 8 && !stoppingToken.IsCancellationRequested; attempt++)
        {
            try
            {
                await using var scope = _services.CreateAsyncScope();
                var db = scope.ServiceProvider.GetRequiredService<FarmDbContext>();
                await SeedData.InitializeAsync(db, stoppingToken);
                _logger.LogInformation("Database migration and seed completed.");
                return;
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                var delay = TimeSpan.FromSeconds(Math.Min(10 * attempt, 60));
                _logger.LogWarning(ex, "Database initialization attempt {Attempt} failed. Retrying in {DelaySeconds}s.", attempt, delay.TotalSeconds);
                await Task.Delay(delay, stoppingToken);
            }
        }

        _logger.LogError("Database initialization did not complete after all retry attempts.");
    }
}
