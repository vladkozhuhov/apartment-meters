FROM node:18-alpine

# Устанавливаем рабочую директорию
WORKDIR /app

# Копируем все файлы проекта
COPY . .

# Устанавливаем только production зависимости 
# (без повторной сборки, так как .next уже построен)
RUN npm ci --omit=dev

# Копируем Docker-специфичные переменные окружения
RUN if [ -f .env.docker ]; then cp .env.docker .env; fi

# Настройка рабочей среды
ENV NODE_ENV=production
EXPOSE 3000

# Запускаем Next.js в production режиме
CMD ["npm", "start"] 
