using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Contexts;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
    public DbSet<UserEntity> Users { get; set; } = null!;
    public DbSet<MeterReadingEntity> MeterReadings { get; set; } = null!;
    public DbSet<AdminAction> AdminActions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserEntity>()
            .HasMany(u => u.MeterReadings)
            .WithOne(mr => mr.UserEntity)
            .HasForeignKey(mr => mr.UserId);

        modelBuilder.Entity<AdminAction>()
            .HasOne(aa => aa.Admin)
            .WithMany()
            .HasForeignKey(aa => aa.AdminId);
    }
}