#Обновить/создать бд
dotnet ef database update --project apartment-meters.Persistence --startup-project apartment-meters.API

#Добавить миграцию
dotnet ef migrations add InitialCreate -c ApplicationDbContext --project apartment-meters.Persistence --startup-project apartment-meters.API --output-dir Migrations

#Удалить миграцию
dotnet ef migrations remove






#Добавление пользователя
{
  "apartmentNumber": 128,
  "fullName": "ГАГАБУДЖА",
  "password": "128128",
  "phoneNumber": "1234123443",
  "role": 0
}