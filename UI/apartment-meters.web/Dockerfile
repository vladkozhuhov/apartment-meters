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

# Копируем .env.docker и переименовываем
COPY .env.docker .env

# Копируем конфигурационные файлы API
COPY .env-configs/ ./.env-configs/
COPY config/ ./config/
COPY setup-api-config.sh ./setup-api-config.sh

# Настраиваем API для продакшен окружения
RUN chmod +x ./setup-api-config.sh && ./setup-api-config.sh production

# Устанавливаем только зависимости, необходимые для запуска
RUN npm ci --omit=dev --no-audit --no-fund --ignore-scripts --prefer-offline \
    && npm cache clean --force

# Копируем только необходимые для запуска файлы
COPY .next ./.next
COPY public ./public
COPY next.config.js ./

# Настройка рабочей среды
ENV NODE_ENV=production
EXPOSE 3000

# Запускаем Next.js в production режиме
CMD ["npm", "start"] 
