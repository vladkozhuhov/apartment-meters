# Apartment Meters

> Modern utility meter management system for apartment buildings with user-friendly interface and reliable backend.

[![CI Status](https://github.com/vladkozhuhov/apartment-meters/actions/workflows/ci.yml/badge.svg)](https://github.com/vladkozhuhov/apartment-meters/actions/workflows/ci.yml)

## 📋 Project Description

Apartment Meters is a full-featured system for managing utility meters in apartment buildings. The application allows residents to easily submit water meter readings, while administrators can monitor resource consumption and manage user accounts.

### Key Features

- 🔐 **Authentication** — secure login system
- 📊 **Meter Reading Management** — submission, storage, and tracking of readings
- 👤 **User Management** — creation and editing of user profiles
- 📱 **Responsive Interface** — works on mobile devices and desktops

## 🛠️ Technology Stack

### Frontend
- **React** — JavaScript library for building user interfaces
- **TypeScript** — strongly typed programming language

### Backend
- **ASP.NET Core** — cross-platform framework for building modern APIs
- **Entity Framework Core** — ORM for database operations
- **PostgreSQL** — open-source relational database
- **JWT Authentication** — secure authentication and authorization
- **AutoMapper** — object mapping
- **FluentValidation** — input validation

### Infrastructure
- **Docker** — application containerization
- **GitHub Actions** — CI/CD processes
- **xUnit** — unit testing
- **Swagger** — API documentation

## 🏗️ Architecture

The project is built using clean architecture principles:

```
API/
├── apartment-meters.API           # Presentation layer (Controllers, Middleware)
├── apartment-meters.Application   # Application layer (Use Cases, DTOs, Validators)
├── apartment-meters.Domain        # Domain layer (Entities, Repositories interfaces)
├── apartment-meters.Infrastructure# Infrastructure layer (External services)
├── apartment-meters.Persistence   # Data access layer (EF Core, Repositories)
└── apartment-meters.Tests         # Unit and integration tests
```

## 🚀 Getting Started

### Prerequisites

- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)
- [.NET SDK 8.0](https://dotnet.microsoft.com/download) (for local development)
- [Node.js](https://nodejs.org/) version 18+ (for local development)

### Running with Docker (recommended)

1. Clone the repository:
   ```bash
   git clone https://github.com/vladkozhuhov/apartment-meters.git
   cd apartment-meters
   ```

2. Start the application using Docker Compose:
   ```bash
   docker-compose up
   ```

### Local Development Setup

#### Backend

1. Navigate to the API directory:
   ```bash
   cd apartment-meters/API
   ```

2. Restore dependencies and run the project:
   ```bash
   dotnet restore
   dotnet run --project apartment-meters.API
   ```

#### Frontend

1. Navigate to the UI directory:
   ```bash
   cd apartment-meters/UI
   ```

2. Install dependencies and start the application:
   ```bash
   npm install
   npm start
   ```

## 👨‍💻 For Developers

### Branching Strategy

- `main` — main branch containing stable release code
- `develop` — development branch containing code for the next release

### Creating New Features

1. Create a new branch from `develop`:
   ```bash
   git checkout develop
   git pull
   git checkout -b feature/my-feature
   ```

2. Implement the feature and create a pull request to `develop`

### CI/CD

The project uses GitHub Actions for automation:

- **CI Pipeline** (.github/workflows/ci.yml) — runs on every push and PR
  - Building the project
  - Running tests
  - Code quality checks

- **CD Pipeline** (.github/workflows/cd.yml) — runs after successful CI on main branch
  - Building and publishing artifacts
  - Deploying the application to staging environment

- **Release Pipeline** (.github/workflows/release.yml) — runs when a version tag is created
  - Publishing a release on GitHub
  - Creating release artifacts

Detailed CI/CD documentation is available in [.github/CI_CD_README.md](.github/CI_CD_README.md)

## Структура веток проекта

Проект использует упрощенную структуру Git-веток:

- `main` - продакшн ветка, содержит стабильный и готовый к выпуску код
- `develop` - ветка разработки, содержит текущие изменения и новый функционал

### Работа с ветками

1. **Разработка:**
   ```bash
   # Переключиться на ветку разработки
   git checkout develop
   
   # Внести изменения, добавить и закоммитить
   git add .
   git commit -m "feat: описание изменений"
   
   # Отправить изменения в репозиторий
   git push origin develop
   ```

2. **Выпуск в продакшн:**
   ```bash
   # Переключиться на ветку main
   git checkout main
   
   # Влить изменения из develop
   git merge develop
   
   # Отправить изменения в репозиторий
   git push origin main
   ```

### Настройка окружения

Проект содержит файлы конфигурации для разных сред:
- `.env.development` - для ветки develop
- `.env.production` - для ветки main

Для применения конфигурации в зависимости от текущей ветки выполните:
```bash
./choose-env.sh
```

Это создаст файл `.env.current` с правильной конфигурацией для текущей ветки.

### Локальная разработка с Docker

Для локальной разработки используйте специальный Docker Compose файл для разработки:

```bash
# Запуск окружения разработки
docker-compose -f docker-compose.dev.yml up -d

# Остановка окружения разработки
docker-compose -f docker-compose.dev.yml down
```

Особенности разработки с Docker:

1. **API доступно по адресу:** http://localhost:5000
2. **Фронтенд доступен по адресу:** http://localhost:3000
3. **База данных PostgreSQL:** доступна на порту 5432

Для работы с базой данных можно использовать любой совместимый клиент:
- Хост: localhost
- Порт: 5432
- Пользователь: postgres
- Пароль: 12341234
- База данных: apartmentmetersdb

#### Горячая перезагрузка

В режиме разработки используется горячая перезагрузка:
- Для API: изменения в C# коде автоматически вызывают перезапуск через dotnet watch
- Для фронтенда: изменения в React/Next.js компонентах обновляются без перезапуска контейнера
