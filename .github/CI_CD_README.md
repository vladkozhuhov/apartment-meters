# CI/CD документация для проекта Apartment Meters

В этом проекте настроена система непрерывной интеграции и развертывания (CI/CD) с использованием GitHub Actions. Система автоматизирует следующие процессы:

## Непрерывная интеграция (CI)

Файл: `.github/workflows/ci.yml`

Этот workflow автоматически запускается при:
- Пуше в ветки `main`, `master` или `develop`
- Создании Pull Request в эти ветки

Шаги CI процесса:
1. Проверка кода из репозитория
2. Настройка .NET SDK
3. Установка необходимых инструментов (.NET tools)
4. Проверка качества кода (code quality)
5. Сборка проекта
6. Запуск тестов с анализом покрытия кода
7. Публикация приложения (только при пушах в main/master)
8. Загрузка артефактов сборки для последующего использования

## Непрерывное развертывание (CD)

Файл: `.github/workflows/cd.yml`

Этот workflow запускается автоматически после успешного выполнения CI в ветках `main` или `master`.

Шаги CD процесса:
1. Загрузка артефактов из CI процесса
2. Распаковка артефактов
3. Деплой в тестовую/staging среду (симуляция)
4. Оповещение о деплое

В реальной ситуации здесь должны быть настроены шаги для деплоя в конкретную среду (облачный провайдер, Docker, Kubernetes и т.д.).

## Создание релизов

Файл: `.github/workflows/release.yml`

Этот workflow запускается автоматически при создании тега, начинающегося с 'v' (например, v1.0.0).

Шаги процесса создания релиза:
1. Проверка кода из репозитория
2. Настройка .NET SDK
3. Сборка и тестирование проекта
4. Публикация API
5. Создание ZIP-архива с релизом
6. Генерация списка изменений на основе коммитов между текущим и предыдущим тегами
7. Создание релиза на GitHub
8. Загрузка ZIP-архива как ассета релиза

## Обновление версии

Файл: `.github/workflows/version-bump.yml`

Этот workflow запускается вручную через GitHub Actions UI, позволяя выбрать тип обновления версии (major, minor, patch).

Шаги процесса обновления версии:
1. Определение текущей версии из последнего тега
2. Расчет новой версии на основе выбранного типа обновления
3. Обновление версии в файлах проекта (.csproj)
4. Коммит изменений, создание тега и пуш в репозиторий

## Использование CI/CD

### Для разработчиков

- Разрабатывайте в feature-ветках
- Создавайте Pull Request в `develop` для проверки кода и тестирования
- После слияния в `develop` система автоматически проверит код
- Для выпуска новой версии запустите workflow `Version Bump` и выберите тип обновления

### Для администраторов

- Следите за результатами выполнения CI/CD в разделе "Actions" на GitHub
- Проверяйте отчеты о покрытии кода
- Используйте автоматически созданные релизы для документирования изменений

## Расширение функциональности

При необходимости можно расширить CI/CD процесс:

1. Добавить автоматизированное тестирование UI
2. Настроить реальный деплой в облачную среду (Azure, AWS, GCP)
3. Добавить шаги для создания и публикации Docker-образов
4. Интегрировать инструменты статического анализа кода (SonarQube, и т.д.) 