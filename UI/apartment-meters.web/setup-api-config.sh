#!/bin/bash

# Скрипт для настройки конфигурации API в зависимости от окружения
# Запускается перед сборкой или запуском приложения

# Определение окружения
ENV="${1:-development}"

# Пути к файлам
CONFIG_DIR=".env-configs"
TARGET_FILE="config/api.ts"
SOURCE_FILE=""

# Определение исходного файла на основе окружения
case "$ENV" in
  "production")
    SOURCE_FILE="$CONFIG_DIR/api.ts.production"
    echo "Настройка API для продакшена"
    ;;
  "development"|*)
    SOURCE_FILE="$CONFIG_DIR/api.ts.develop"
    echo "Настройка API для разработки"
    ;;
esac

# Проверка наличия исходного файла
if [ ! -f "$SOURCE_FILE" ]; then
  echo "Ошибка: файл $SOURCE_FILE не найден"
  exit 1
fi

# Копирование файла конфигурации
cp "$SOURCE_FILE" "$TARGET_FILE"
echo "Конфигурация API успешно установлена из $SOURCE_FILE в $TARGET_FILE"

# Вывод информации о конфигурации
echo "Текущая конфигурация API:"
cat "$TARGET_FILE" | grep API_BASE_URL

exit 0 