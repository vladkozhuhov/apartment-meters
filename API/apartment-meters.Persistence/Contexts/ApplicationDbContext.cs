using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Contexts;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<WaterMeterReading> WaterMeterReadings { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().HasIndex(u => u.ApartmentNumber).IsUnique();
    }
}