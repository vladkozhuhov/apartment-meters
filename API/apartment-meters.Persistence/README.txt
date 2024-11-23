#обновить/создать бд
dotnet ef database update -c apartment-meters.API.Persistence.Contexts.ApplicationDbContext --project apartment-meters.API

#добавить миграцию
dotnet ef migrations add CreateCategoryTable -c apartment-meters.API.Persistence.Contexts.ApplicationDbContext --project apartment-meters.API
dotnet ef migration add CreateCategoryTable

#удалить миграцию
dotnet ef migrations remove