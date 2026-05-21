using domain.Events;
using domain.Repositories;
using infrastructure.Context;
using infrastructure.Database;
using infrastructure.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
namespace infrastructure.Configurations;

public static class DependencyInjection
{
    public static void AddInfrastructure( this IServiceCollection Services)
    {
        // DbContext
        Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite("DataSource=srp-example.db")
            );
        // Repositories
        Services.AddScoped<IPlanRepository, PlanRepository>();
        Services.AddScoped<IServiceRepository, ServiceRepository>();
        Services.AddScoped<IFeatureRepository, FeatureRepository>();
        // UnitOfWork
        Services.AddScoped<IUnitOfWork, UnitOfWork>();
        // Event Dispatcher
        Services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();
    }

    public static void InitializeDatabase(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();
    }
}
