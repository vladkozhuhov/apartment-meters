#Обновить/создать бд
dotnet ef database update --project apartment-meters.Persistence --startup-project apartment-meters.API

#Добавить миграцию
dotnet ef migrations add InitialCreate -c ApplicationDbContext --project apartment-meters.Persistence --startup-project apartment-meters.API --output-dir Migrations

#Удалить миграцию
dotnet ef migrations remove

#Docker
docker-compose -f ./docker-compose.yml up





#Добавление пользователя
{
  "apartmentNumber": 90,
  "fullName": "ГАГАБУДЖА",
  "password": "128128",
  "phoneNumber": "1234123443",
  "role": 0
}
{
  "apartmentNumber": 33,
  "fullName": "лдлаовылдлыв",
  "password": "128128",
  "phoneNumber": "1234123443",
  "role": 0
}
{
  "apartmentNumber": 55,
  "fullName": "1234пппп",
  "password": "128128",
  "phoneNumber": "1234123443",
  "role": 0
}
{
  "apartmentNumber": 120,
  "fullName": "ПУПУПУ",
  "password": "128128",
  "phoneNumber": "1234123443",
  "role": 0
}