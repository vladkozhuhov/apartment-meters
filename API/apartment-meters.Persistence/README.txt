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
  "apartmentNumber": 900,
  "lastName": "Петров",
  "firstName": "Михаил",
  "middleName": "Анатольевич",
  "password": "12341234",
  "phoneNumber": "89513009780",
  "role": 0,
  "factoryNumber": "90090090",
  "factoryYear": "2024-07-10T17:28:56.790Z"
}
{
  "apartmentNumber": 901,
  "lastName": "Админин",
  "firstName": "Админ",
  "middleName": "Админович",
  "password": "12341234",
  "phoneNumber": "89027832605",
  "role": 1,
  "factoryNumber": "90190190",
  "factoryYear": "2025-01-03T17:28:56.790Z"
}