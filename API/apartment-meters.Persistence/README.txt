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
  "role": 0
}
{
  "apartmentNumber": 905,
  "lastName": "Ашотов",
  "firstName": "Генадий",
  "middleName": "Палыч",
  "password": "12341234",
  "phoneNumber": "89513009780",
  "role": 0
}

# Добавление счетчиков для пользователя
{
  "userId": "6d462205-0b5f-4053-9b9a-daee0b057e35",
  "placeOfWaterMeter": 0,
  "waterType": 0,
  "factoryNumber": "90090091",
  "factoryYear": "2025-02-28"
},
{
  "userId": "6d462205-0b5f-4053-9b9a-daee0b057e35",
  "placeOfWaterMeter": 0,
  "waterType": 1,
  "factoryNumber": "90090092",
  "factoryYear": "2024-07-10T17:28:56.790Z"
},
{
   "userId": "6d462205-0b5f-4053-9b9a-daee0b057e35",
   "placeOfWaterMeter": 1,
   "waterType": 0,
   "factoryNumber": "90090093",
  "factoryYear": "2024-07-10T17:28:56.790Z"
},
{
    "userId": "6d462205-0b5f-4053-9b9a-daee0b057e35",
    "placeOfWaterMeter": 1,
    "waterType": 1,
    "factoryNumber": "90090094",
  "factoryYear": "2024-07-10T17:28:56.790Z"
}

# Добавление админа
{
  "apartmentNumber": 901,
  "lastName": "Админин",
  "firstName": "Админ",
  "middleName": "Админович",
  "password": "12341234",
  "phoneNumber": "89027832605",
  "role": 1
}

# Данные всех пользователей
[
  {
    "id": "0b98b452-d14a-4226-96da-8335c0afcda9",
    "apartmentNumber": 900,
    "lastName": "Петров",
    "firstName": "Михаил",
    "middleName": "Анатольевич",
    "password": "12341234",
    "phoneNumber": "89513009780",
    "role": 0,
    "createdAt": "2025-02-28T12:08:38.773462Z",
    "updatedAt": "2025-02-28T12:08:38.773462Z",
    "waterMeters": []
  },
  {
    "id": "69bd4927-40ee-48a4-a33c-4eedc60e853e",
    "apartmentNumber": 901,
    "lastName": "Админин",
    "firstName": "Админ",
    "middleName": "Админович",
    "password": "12341234",
    "phoneNumber": "89027832605",
    "role": 1,
    "createdAt": "2025-02-28T12:09:34.133025Z",
    "updatedAt": "2025-02-28T12:09:34.133025Z",
    "waterMeters": []
  }
]