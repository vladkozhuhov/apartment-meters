// Версия кеша
const CACHE_VERSION = 'v1.2';
const CACHE_NAME = `apartment-meters-cache-${CACHE_VERSION}`;

// Ресурсы, которые будут кешироваться
const CACHED_RESOURCES = [
  '/',
  '/index.html',
  '/favicon.ico',
  '/manifest.json',
  // Иконки
  '/vercel.svg',
  '/next.svg',
  '/globe.svg',
  '/window.svg',
  '/file.svg',
  // Динамические пути для Next.js
  // (будут добавлены во время выполнения)
];

// Событие установки - кешируем статические ресурсы
self.addEventListener('install', (event) => {
  console.log('[SW] Установка Service Worker');
  
  // Принудительно активируем сервис-воркер без ожидания закрытия вкладок
  self.skipWaiting()
    .then(() => console.log('[SW] Сервис-воркер активирован (skipWaiting)'))
    .catch(error => console.error('[SW] Ошибка при skipWaiting:', error));
  
  event.waitUntil(
    caches.open(CACHE_NAME)
      .then((cache) => {
        console.log('[SW] Кеширование ресурсов:', CACHED_RESOURCES);
        return cache.addAll(CACHED_RESOURCES);
      })
      .then(() => {
        // Запрашиваем разрешение на показ уведомлений при установке
        if (self.Notification && self.Notification.permission !== 'granted') {
          console.log('[SW] Запрашиваем разрешение на показ уведомлений');
        }
        
        console.log('[SW] Установка сервис-воркера завершена');
      })
      .catch(error => {
        console.error('[SW] Ошибка при установке сервис-воркера:', error);
      })
  );
});

// Событие активации - удаляем старые версии кеша
self.addEventListener('activate', (event) => {
  console.log('[SW] Активация Service Worker');
  
  event.waitUntil(
    caches.keys().then(async (cacheNames) => {
      // Удаляем старые версии кеша
      const cachesToDelete = cacheNames.filter(cacheName => 
        cacheName.startsWith('apartment-meters-cache-') && cacheName !== CACHE_NAME
      );
      
      await Promise.all(cachesToDelete.map(cacheName => {
        console.log('[SW] Удаление старого кеша:', cacheName);
        return caches.delete(cacheName);
      }));
      
      // Заявляем контроль над всеми клиентами
      console.log('[SW] Service Worker активирован');
      return self.clients.claim();
    })
  );
});

// Стратегия cache-first для статического контента
self.addEventListener('fetch', (event) => {
  // Не перехватываем запросы к API или аналитике
  if (event.request.url.includes('/api/') || 
      event.request.url.includes('google-analytics') ||
      event.request.method !== 'GET') {
    return;
  }

  event.respondWith(
    caches.match(event.request)
      .then((cachedResponse) => {
        // Если ресурс в кеше, возвращаем его
        if (cachedResponse) {
          return cachedResponse;
        }
        
        // Если ресурса нет в кеше, получаем его из сети
        return fetch(event.request)
          .then((response) => {
            // Проверяем, что ответ корректный
            if (!response || response.status !== 200 || response.type !== 'basic') {
              return response;
            }
            
            // Клонируем ответ, чтобы сохранить его в кеше
            const responseToCache = response.clone();
            
            // Добавляем ответ в кеш
            caches.open(CACHE_NAME)
              .then((cache) => {
                // Не кешируем страницы с параметрами
                if (!event.request.url.includes('?')) {
                  cache.put(event.request, responseToCache);
                }
              });
            
            return response;
          })
          .catch((error) => {
            console.error('[SW] Ошибка при загрузке ресурса:', error);
            // Для HTML-страниц можно вернуть страницу оффлайн
            if (event.request.headers.get('accept').includes('text/html')) {
              return caches.match('/');
            }
            return new Response('Сервис недоступен', {
              status: 503,
              statusText: 'Service Unavailable'
            });
          });
      })
  );
});

// Обработка push-уведомлений с большей надежностью
self.addEventListener('push', (event) => {
  console.log('[SW] Получено push-уведомление');
  
  // Принудительно будим сервис-воркер для обработки события
  self.skipWaiting()
    .then(() => console.log('[SW] Сервис-воркер активирован для push-уведомления'))
    .catch(() => console.log('[SW] Не удалось принудительно активировать сервис-воркер'));
  
  let payload;
  
  // Безопасно пытаемся распарсить данные уведомления
  try {
    if (event.data) {
      payload = event.data.json();
    } else {
      payload = {
        title: 'Уведомление от Apartment Meters',
        body: 'Система учета показаний счетчиков',
        url: '/'
      };
    }
  } catch (error) {
    console.error('[SW] Ошибка при разборе данных уведомления:', error);
    // Используем данные по умолчанию
    payload = {
      title: 'Новое уведомление',
      body: 'Проверьте информацию в приложении',
      url: '/'
    };
  }
  
  console.log('[SW] Данные уведомления:', payload);
  
  const title = payload.title || 'Уведомление';
  const options = {
    body: payload.body || 'Новое уведомление',
    icon: payload.icon || '/vercel.svg',
    badge: '/vercel.svg',
    data: {
      ...payload.data,
      url: payload.url || '/'
    },
    requireInteraction: true,
    vibrate: [100, 50, 100],
    actions: payload.actions || []
  };

  // Показываем уведомление даже если что-то пошло не так
  event.waitUntil(
    self.registration.showNotification(title, options)
      .then(() => {
        console.log('[SW] Уведомление отображено');
      })
      .catch(error => {
        console.error('[SW] Ошибка при отображении уведомления:', error);
      })
  );
});

// Обработка клика по уведомлению
self.addEventListener('notificationclick', (event) => {
  console.log('[SW] Клик по уведомлению', event.notification.data);
  
  // Закрываем уведомление
  event.notification.close();
  
  // Получаем URL для открытия из данных уведомления или по умолчанию '/'
  const urlToOpen = event.notification.data?.url || '/';
  
  // Проверяем, какое действие было выбрано (если доступны действия)
  if (event.action) {
    console.log('[SW] Выбрано действие:', event.action);
    // Здесь можно добавить специальную логику для разных действий
  }

  // Открываем или фокусируемся на окне с указанным URL
  event.waitUntil(
    self.clients.matchAll({ type: 'window' })
      .then((clientList) => {
        // Ищем открытое окно с нашим приложением
        for (const client of clientList) {
          if (client.url.includes(self.registration.scope) && 'focus' in client) {
            // Если окно уже открыто, фокусируемся на нем и навигируем
            client.postMessage({
              type: 'NOTIFICATION_CLICK',
              url: urlToOpen
            });
            
            return client.focus();
          }
        }
        
        // Если окно не найдено, открываем новое
        if (self.clients.openWindow) {
          return self.clients.openWindow(urlToOpen);
        }
      })
      .catch(error => {
        console.error('[SW] Ошибка при обработке клика по уведомлению:', error);
      })
  );
});

// Отправка сообщений от сервис-воркера клиентам
const notifyClients = (message) => {
  self.clients.matchAll()
    .then(clients => {
      clients.forEach(client => {
        client.postMessage(message);
      });
    });
};

// Синхронизация в фоне
self.addEventListener('sync', (event) => {
  if (event.tag === 'sync-data') {
    console.log('[SW] Выполняется фоновая синхронизация');
    
    // Здесь будет логика фоновой синхронизации
    notifyClients({ type: 'SYNC_COMPLETED' });
  }
});

// Обработка ошибок
self.addEventListener('error', (event) => {
  console.error('[SW] Необработанная ошибка в сервис-воркере:', event.error);
});

// Этот обработчик выполняется, когда сервис-воркер получает сообщение от клиентов
self.addEventListener('message', (event) => {
  console.log('[SW] Получено сообщение от клиента:', event.data);
  
  if (event.data && event.data.type === 'SKIP_WAITING') {
    console.log('[SW] Получена команда на принудительную активацию');
    // Принудительно активируем текущую версию сервис-воркера
    self.skipWaiting()
      .then(() => {
        console.log('[SW] Сервис-воркер успешно активирован по запросу клиента');
        // Уведомляем клиентов об успешной активации
        self.clients.matchAll().then(clients => {
          clients.forEach(client => {
            client.postMessage({ type: 'ACTIVATED' });
          });
        });
      })
      .catch(error => {
        console.error('[SW] Ошибка при принудительной активации:', error);
      });
  }
  
  if (event.data && event.data.type === 'CHECK_STATE') {
    // Отправляем текущее состояние сервис-воркера клиенту
    const state = {
      type: 'SW_STATE',
      state: self.registration.installing ? 'installing' :
             self.registration.waiting ? 'waiting' :
             self.registration.active ? 'active' : 'unknown',
      version: CACHE_VERSION
    };
    
    if (event.source) {
      event.source.postMessage(state);
    }
  }
}); 