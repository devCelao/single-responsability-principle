using domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace infrastructure.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Plan> Plans { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Feature> Features { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Plan>()
            .HasMany(p => p.Services)
            .WithMany();

        modelBuilder.Entity<Service>()
            .HasMany(s => s.Features)
            .WithMany();

        base.OnModelCreating(modelBuilder);
    }
}
