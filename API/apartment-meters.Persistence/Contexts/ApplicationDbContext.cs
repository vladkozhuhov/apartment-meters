using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Contexts;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<MeterReading> MeterReadings { get; set; } = null!;
    public DbSet<AdminAction> AdminActions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasMany(u => u.MeterReadings)
            .WithOne(mr => mr.User)
            .HasForeignKey(mr => mr.UserId);

        modelBuilder.Entity<AdminAction>()
            .HasOne(aa => aa.Admin)
            .WithMany()
            .HasForeignKey(aa => aa.AdminId);
    }
}