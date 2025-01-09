using Domain.Entities;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace API.Helpers;

/// <summary>
/// Вспомогательный класс для EDM OData
/// </summary>
public class EdmHelper
{
    /// <summary>
    /// Получение модели EDM
    /// </summary>
    public static IEdmModel GetEdmModel()
    {
        ODataConventionModelBuilder builder = new();

        EntityTypeConfiguration<UserEntity> user = builder
            .EntitySet<UserEntity>("user")
            .EntityType
            .HasKey(x => x.Id);
        
        EntityTypeConfiguration<MeterReadingEntity> meterReading = builder
            .EntitySet<MeterReadingEntity>("meterReading")
            .EntityType
            .HasKey(x => x.Id);
        
        return builder.GetEdmModel();
    }
}