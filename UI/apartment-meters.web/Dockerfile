FROM node:18-alpine AS base

# Устанавливаем рабочую директорию
WORKDIR /app

# Копируем файлы package.json и package-lock.json
COPY package.json package-lock.json ./

# Устанавливаем зависимости
RUN npm ci

# Копируем остальные файлы проекта
COPY . .

# Копируем Docker-специфичные переменные окружения
COPY .env.docker .env

# Создаем production сборку
RUN npm run build

# Запускаем проект в production режиме
ENV NODE_ENV=production
EXPOSE 3000

CMD ["npm", "start"] 
