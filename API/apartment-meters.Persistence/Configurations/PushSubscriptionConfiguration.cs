using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

/// <summary>
/// Конфигурация для сущности PushSubscriptionEntity
/// </summary>
public class PushSubscriptionConfiguration : IEntityTypeConfiguration<PushSubscriptionEntity>
{
    /// <summary>
    /// Настраивает сущность PushSubscriptionEntity для EntityFramework
    /// </summary>
    /// <param name="builder">Построитель конфигурации типа сущности</param>
    public void Configure(EntityTypeBuilder<PushSubscriptionEntity> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Endpoint)
            .IsRequired()
            .HasMaxLength(512);
            
        builder.Property(p => p.P256dh)
            .IsRequired()
            .HasMaxLength(512);
            
        builder.Property(p => p.Auth)
            .IsRequired()
            .HasMaxLength(512);
            
        builder.Property(p => p.DeviceType)
            .HasMaxLength(50);
            
        builder.Property(p => p.CreatedAt)
            .IsRequired();
            
        builder.HasIndex(p => p.Endpoint)
            .IsUnique();
            
        builder.HasIndex(p => p.UserId);
            
        // Связь с пользователем
        builder.HasOne(p => p.User)
            .WithMany() // Предполагается, что у UserEntity нет обратной навигации
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 