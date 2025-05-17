FROM node:18-alpine AS builder

# Оптимизация npm
ENV NPM_CONFIG_LOGLEVEL=error
ENV NODE_OPTIONS="--max-old-space-size=2048"
ENV npm_config_cache=/tmp/npm-cache
ENV npm_config_userconfig=/tmp/.npmrc

# Устанавливаем рабочую директорию
WORKDIR /app

# Копируем только необходимые файлы для установки пакетов
COPY package.json package-lock.json ./

# Копируем файлы окружения
COPY .env.docker ./.env

# Устанавливаем только зависимости, необходимые для запуска
RUN npm ci --omit=dev --no-audit --no-fund --ignore-scripts --prefer-offline \
    && npm cache clean --force

# Копируем только необходимые для запуска файлы
COPY .next ./.next
COPY public ./public
COPY next.config.js ./

# Создаем директорию config и api.ts файл с пустым API_BASE_URL для продакшена
RUN mkdir -p config && \
    echo 'export const API_BASE_URL = ""; export const API_TIMEOUT = 15000; export const API_MAX_RETRIES = 1; export const API_DEFAULT_HEADERS = {"Content-Type": "application/json", "Accept": "application/json"};' > config/api.ts

# Настройка рабочей среды
ENV NODE_ENV=production
EXPOSE 3000

# Запускаем Next.js в production режиме
CMD ["npm", "start"] 
