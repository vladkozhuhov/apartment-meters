#!/bin/bash

# Скрипт для настройки окружения проекта
# Позволяет переключаться между dev и prod конфигурациями

# Получение текущей директории скрипта
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
UI_DIR="$SCRIPT_DIR/UI/apartment-meters.web"

# Функция вывода справки
show_help() {
    echo "Использование: $0 [опции]"
    echo "Опции:"
    echo "  -e, --env ENV       Установить окружение (development или production)"
    echo "  -h, --help          Показать эту справку"
    echo ""
    echo "Пример: $0 --env production"
}

# Проверка аргументов
ENV="development"

while [[ $# -gt 0 ]]; do
    case "$1" in
        -e|--env)
            ENV="$2"
            shift 2
            ;;
        -h|--help)
            show_help
            exit 0
            ;;
        *)
            echo "Неизвестная опция: $1"
            show_help
            exit 1
            ;;
    esac
done

# Проверка правильности указанного окружения
if [[ "$ENV" != "development" && "$ENV" != "production" ]]; then
    echo "Ошибка: неправильное окружение '$ENV'. Используйте 'development' или 'production'."
    exit 1
fi

echo "Настройка окружения: $ENV"

# Настройка API для UI
if [ -f "$UI_DIR/setup-api-config.sh" ]; then
    echo "Настройка API для UI..."
    (cd "$UI_DIR" && ./setup-api-config.sh "$ENV")
else
    echo "Ошибка: файл $UI_DIR/setup-api-config.sh не найден"
    exit 1
fi

echo "Окружение успешно настроено на: $ENV"
exit 0 