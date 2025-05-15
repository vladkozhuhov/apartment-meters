#!/bin/bash

# Скрипт для настройки окружения разработки
# Создает необходимые директории и файлы для разработки

# Цвета для вывода
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo -e "${YELLOW}Настройка окружения разработки...${NC}"

# 1. Проверка текущей ветки
CURRENT_BRANCH=$(git symbolic-ref --short HEAD 2>/dev/null)
if [ $? -ne 0 ]; then
    echo -e "${RED}Ошибка: Текущий каталог не является Git репозиторием${NC}"
    exit 1
fi

echo -e "${GREEN}Текущая ветка: ${CURRENT_BRANCH}${NC}"

# 2. Создание директорий для Docker
echo -e "${YELLOW}Создание директорий для Docker...${NC}"
mkdir -p nginx/conf.d
mkdir -p certbot/conf
mkdir -p certbot/www
mkdir -p nginx/auth

# 3. Применение конфигурации для текущей среды
echo -e "${YELLOW}Применение конфигурации для текущей среды...${NC}"
if [ "$CURRENT_BRANCH" = "main" ]; then
    echo -e "${GREEN}Установка продакшн окружения${NC}"
    cp .env.production .env.current
elif [ "$CURRENT_BRANCH" = "develop" ]; then
    echo -e "${GREEN}Установка окружения разработки${NC}"
    cp .env.development .env.current
else
    echo -e "${YELLOW}Ветка не распознана, используем конфигурацию для разработки${NC}"
    cp .env.development .env.current
fi

# 4. Создание nginx конфигурации для разработки
if [ "$CURRENT_BRANCH" = "develop" ]; then
    echo -e "${YELLOW}Создание nginx конфигурации для разработки...${NC}"
    cat > nginx/conf.d/default.conf <<EOL
server {
    listen 80;
    server_name localhost;

    location /api/ {
        proxy_pass http://api:8080/;
        proxy_http_version 1.1;
        proxy_set_header Upgrade \$http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host \$host;
        proxy_cache_bypass \$http_upgrade;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;
    }

    location / {
        proxy_pass http://frontend:3000/;
        proxy_http_version 1.1;
        proxy_set_header Upgrade \$http_upgrade;
        proxy_set_header Connection "upgrade";
        proxy_set_header Host \$host;
        proxy_cache_bypass \$http_upgrade;
    }
}
EOL
    echo -e "${GREEN}Nginx конфигурация создана${NC}"
fi

echo -e "${GREEN}Настройка окружения завершена успешно!${NC}"
echo -e "${YELLOW}Для запуска окружения разработки выполните:${NC}"
echo -e "docker-compose -f docker-compose.dev.yml up -d" 