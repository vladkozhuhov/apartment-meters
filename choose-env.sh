#!/bin/bash

# Скрипт для определения и установки правильных переменных окружения
# в зависимости от текущей ветки Git

# Получаем текущую ветку
CURRENT_BRANCH=$(git symbolic-ref --short HEAD)
ENV_FILE=".env.current"

echo "Определение окружения для ветки: $CURRENT_BRANCH"

# Выбираем файл конфигурации в зависимости от ветки
if [ "$CURRENT_BRANCH" = "main" ]; then
    echo "Установка продакшн окружения"
    cp .env.production $ENV_FILE
elif [ "$CURRENT_BRANCH" = "develop" ]; then
    echo "Установка окружения разработки"
    cp .env.development $ENV_FILE
else
    echo "Ветка не распознана, используем конфигурацию для разработки"
    cp .env.development $ENV_FILE
fi

echo "Конфигурация успешно обновлена!" 