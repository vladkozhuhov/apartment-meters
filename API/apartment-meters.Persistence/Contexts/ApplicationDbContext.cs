using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Contexts;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<WaterMeterEntity> WaterMeters { get; set; }
    public DbSet<MeterReadingEntity> MeterReadings { get; set; }
    public DbSet<PushSubscriptionEntity> PushSubscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Настройка связи 1 ко многим: пользователь -> счетчики
        modelBuilder.Entity<WaterMeterEntity>()
            .HasOne(wm => wm.User)
            .WithMany(u => u.WaterMeters)
            .HasForeignKey(wm => wm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Настройка связи 1 ко многим: счетчик -> показания
        modelBuilder.Entity<MeterReadingEntity>()
            .HasOne(mr => mr.WaterMeter)
            .WithMany(wm => wm.MeterReadings)
            .HasForeignKey(mr => mr.WaterMeterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}