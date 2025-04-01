import api from './api';

/**
 * Интерфейс подписки на Push-уведомления
 */
export interface PushSubscription {
  endpoint: string;
  keys: {
    p256dh: string;
    auth: string;
  };
  deviceType?: string;
}

/**
 * Сервис для работы с Push-уведомлениями
 * Пользователи получают уведомления автоматически без возможности отказа
 */
class PushNotificationService {
  private swRegistration: ServiceWorkerRegistration | null = null;
  private vapidPublicKey: string | null = null;

  /**
   * Проверяет поддержку Push API в браузере
   */
  isSupported(): boolean {
    return typeof window !== 'undefined' && 
           'serviceWorker' in navigator && 
           'PushManager' in window;
  }

  /**
   * Получает публичный VAPID ключ с сервера
   */
  async getVapidPublicKey(): Promise<string> {
    try {
      if (this.vapidPublicKey) {
        return this.vapidPublicKey;
      }

      const response = await api.get('/api/PushNotification/vapid-public-key');
      this.vapidPublicKey = response.data;
      
      if (!this.vapidPublicKey) {
        throw new Error('Не удалось получить публичный VAPID ключ');
      }
      
      return this.vapidPublicKey;
    } catch (error) {
      console.error('Ошибка при получении VAPID ключа:', error);
      throw error;
    }
  }

  /**
   * Регистрирует сервис-воркер для Push-уведомлений и ждет его активации
   */
  async registerServiceWorker(): Promise<ServiceWorkerRegistration> {
    try {
      // Возвращаем существующую регистрацию, если воркер уже активен
      if (this.swRegistration && this.swRegistration.active) {
        console.log('Используем существующую регистрацию сервис-воркера');
        return this.swRegistration;
      }

      console.log('Начинаем регистрацию сервис-воркера...');
      
      // Проверяем наличие "ready" сервис-воркера
      const readyRegistration = await navigator.serviceWorker.ready
        .catch(() => null);
      
      if (readyRegistration && readyRegistration.active) {
        console.log('Найден уже готовый сервис-воркер');
        this.swRegistration = readyRegistration;
        return readyRegistration;
      }
      
      // Регистрируем сервис-воркер из корневой директории
      this.swRegistration = await navigator.serviceWorker.register('/service-worker.js');
      console.log('Сервис-воркер зарегистрирован:', this.swRegistration);
      
      // Если сервис-воркер уже активен, возвращаем его сразу
      if (this.swRegistration.active) {
        console.log('Сервис-воркер уже активен');
        return this.swRegistration;
      }
      
      // Дожидаемся установки и активации
      if (this.swRegistration.installing || this.swRegistration.waiting) {
        console.log('Ожидаем установку и активацию сервис-воркера...');
        
        await new Promise<void>((resolve, reject) => {
          // Устанавливаем таймаут на ожидание
          const timeout = setTimeout(() => {
            reject(new Error('Таймаут ожидания активации сервис-воркера'));
          }, 10000); // 10 секунд максимально ждем
          
          const stateListener = () => {
            if (this.swRegistration?.active) {
              clearTimeout(timeout);
              console.log('Сервис-воркер успешно активирован');
              
              // Удаляем обработчик после активации
              if (this.swRegistration && this.swRegistration.installing) {
                this.swRegistration.installing.removeEventListener('statechange', stateListener);
              }
              if (this.swRegistration && this.swRegistration.waiting) {
                this.swRegistration.waiting.removeEventListener('statechange', stateListener);
              }
              
              resolve();
            }
          };
          
          // Прослушиваем изменения состояния
          if (this.swRegistration && this.swRegistration.installing) {
            console.log('Прослушиваем состояние установки...');
            this.swRegistration.installing.addEventListener('statechange', stateListener);
            stateListener(); // Проверяем сразу
          }
          
          if (this.swRegistration && this.swRegistration.waiting) {
            console.log('Прослушиваем состояние ожидания...');
            this.swRegistration.waiting.addEventListener('statechange', stateListener);
            stateListener(); // Проверяем сразу
          }
        });
      }
      
      // Дополнительно ждем готовности navigator.serviceWorker.ready
      await navigator.serviceWorker.ready;
      
      console.log('Сервис-воркер полностью готов к использованию');
      return this.swRegistration;
    } catch (error) {
      console.error('Ошибка при регистрации сервис-воркера:', error);
      throw error;
    }
  }

  /**
   * Преобразует base64 строку в Uint8Array для использования в Web Push API
   */
  private urlBase64ToUint8Array(base64String: string): Uint8Array {
    const padding = '='.repeat((4 - (base64String.length % 4)) % 4);
    const base64 = (base64String + padding)
      .replace(/-/g, '+')
      .replace(/_/g, '/');

    const rawData = window.atob(base64);
    const outputArray = new Uint8Array(rawData.length);

    for (let i = 0; i < rawData.length; ++i) {
      outputArray[i] = rawData.charCodeAt(i);
    }
    return outputArray;
  }

  /**
   * Подписывается на Push-уведомления
   * В новой версии - автоматическая подписка без возможности отказа
   */
  async subscribe(): Promise<PushSubscription | null> {
    try {
      if (!this.isSupported()) {
        console.warn('Этот браузер не поддерживает Push API');
        return null;
      }

      // Сначала запрашиваем разрешение на отправку уведомлений, если оно не предоставлено
      if (Notification.permission !== 'granted') {
        console.log('Запрашиваем разрешение на отправку уведомлений...');
        const permission = await Notification.requestPermission();
        
        if (permission !== 'granted') {
          console.warn('Разрешение на отправку уведомлений не получено:', permission);
          return null;
        }
        
        console.log('Получено разрешение на отправку уведомлений');
      }

      console.log('Начинаем процесс подписки на push-уведомления...');
      
      // Регистрируем сервис-воркер и ждем его активации
      const registration = await this.registerServiceWorker();
      
      // Еще раз проверяем, что сервис-воркер активен
      if (!registration.active) {
        console.error('Не удалось активировать сервис-воркер');
        throw new Error('Сервис-воркер не активирован');
      }
      
      console.log('Сервис-воркер активен, продолжаем подписку...');
      
      // Получаем VAPID ключ
      const vapidPublicKey = await this.getVapidPublicKey();
      const convertedVapidKey = this.urlBase64ToUint8Array(vapidPublicKey);

      // Проверяем наличие существующей подписки
      let subscription = await registration.pushManager.getSubscription();
      if (subscription) {
        console.log('Уже есть подписка на уведомления:', subscription);
        return this.convertSubscription(subscription);
      }

      // Создаем новую подписку с несколькими попытками
      console.log('Создаем новую подписку на push-уведомления...');
      
      let attempts = 0;
      const maxAttempts = 3;
      let lastError = null;
      
      while (attempts < maxAttempts) {
        attempts++;
        console.log(`Попытка подписки #${attempts}...`);
        
        try {
          // Небольшая пауза перед подпиской
          await new Promise(resolve => setTimeout(resolve, 500 * attempts));
          
          subscription = await registration.pushManager.subscribe({
            userVisibleOnly: true,
            applicationServerKey: convertedVapidKey
          });
          
          console.log('Успешно создана подписка на push-уведомления:', subscription);
          
          // Сохраняем подписку на сервере
          const pushSubscription = this.convertSubscription(subscription);
          await this.saveSubscription(pushSubscription);
          
          return pushSubscription;
        } catch (error) {
          lastError = error;
          console.warn(`Ошибка при подписке (попытка ${attempts}/${maxAttempts}):`, error);
          
          if (Notification.permission === 'denied') {
            console.error('Пользователь отклонил разрешение на уведомления');
            return null;
          }
        }
      }
      
      // Если все попытки не удались
      console.error(`Не удалось создать подписку после ${maxAttempts} попыток:`, lastError);
      throw lastError || new Error('Не удалось создать подписку на push-уведомления');
    } catch (error) {
      console.error('Критическая ошибка при подписке на push-уведомления:', error);
      return null;
    }
  }

  /**
   * Метод отписки отключен, поскольку пользователи должны всегда получать уведомления
   * Метод оставлен для совместимости с существующим кодом
   */
  async unsubscribe(): Promise<boolean> {
    console.log('Отписка от уведомлений отключена в этой версии');
    return false;
  }

  /**
   * Принудительно активирует сервис-воркер 
   */
  async activateServiceWorker(): Promise<boolean> {
    try {
      if (!('serviceWorker' in navigator)) {
        return false;
      }
      
      // Получаем все регистрации сервис-воркеров
      const registrations = await navigator.serviceWorker.getRegistrations();
      
      if (registrations.length === 0) {
        console.log('Нет зарегистрированных сервис-воркеров');
        return false;
      }
      
      // Проверяем, есть ли ожидающие активации сервис-воркеры
      let hasWaitingWorkers = false;
      
      for (const registration of registrations) {
        if (registration.waiting) {
          hasWaitingWorkers = true;
          
          // Отправляем сообщение, чтобы активировать ожидающий сервис-воркер
          registration.waiting.postMessage({ type: 'SKIP_WAITING' });
          console.log('Отправлено сообщение SKIP_WAITING для активации сервис-воркера');
        }
      }
      
      if (hasWaitingWorkers) {
        // Ждем немного, чтобы сервис-воркер успел активироваться
        await new Promise(resolve => setTimeout(resolve, 1000));
      }
      
      return true;
    } catch (error) {
      console.error('Ошибка при активации сервис-воркера:', error);
      return false;
    }
  }

  /**
   * Простая проверка наличия активной подписки
   * Использует упрощенный подход, чтобы избежать таймаутов
   */
  async hasActiveSubscription(): Promise<boolean> {
    try {
      if (!this.isSupported()) {
        return false;
      }
      
      // Сначала проверяем текущий статус Notification API
      if (Notification.permission !== 'granted') {
        return false;
      }
      
      // Затем проверяем регистрацию 
      const registrations = await navigator.serviceWorker.getRegistrations();
      if (registrations.length === 0) {
        return false;
      }
      
      // Считаем, что разрешение есть, если есть активная регистрация и разрешение в браузере
      return true;
    } catch (error) {
      console.error('Ошибка при проверке активной подписки:', error);
      return false;
    }
  }

  /**
   * Проверяет текущий статус подписки
   */
  async getSubscriptionStatus(): Promise<'subscribed' | 'not-subscribed' | 'not-supported'> {
    if (!this.isSupported()) {
      return 'not-supported';
    }

    try {
      // Сначала проверяем более простым методом
      const hasSubscription = await this.hasActiveSubscription();
      if (hasSubscription) {
        return 'subscribed';
      }
      
      // Если у нас нет разрешения, то точно не подписаны
      if (Notification.permission !== 'granted') {
        return 'not-subscribed';
      }
      
      // Используем таймаут для предотвращения зависания
      const registrationPromise = navigator.serviceWorker.ready;
      
      // Создаем промис с таймаутом
      const timeoutPromise = new Promise<null>((resolve) => {
        setTimeout(() => resolve(null), 1000); // Уменьшаем таймаут до 1 секунды
      });
      
      // Используем Promise.race для ограничения времени ожидания
      const registration = await Promise.race([
        registrationPromise,
        timeoutPromise
      ]);
      
      if (!registration) {
        console.warn('Таймаут при получении регистрации serviceWorker.ready');
        return 'not-subscribed';
      }
      
      try {
        const subscription = await registration.pushManager.getSubscription();
        return subscription ? 'subscribed' : 'not-subscribed';
      } catch (error) {
        console.error('Ошибка при получении подписки:', error);
        return 'not-subscribed';
      }
    } catch (error) {
      console.error('Ошибка при проверке статуса подписки:', error);
      return 'not-subscribed';
    }
  }

  /**
   * Конвертирует объект подписки в формат для отправки на сервер
   */
  private convertSubscription(subscription: PushSubscriptionJSON): PushSubscription {
    if (!subscription.endpoint || !subscription.keys || !subscription.keys.p256dh || !subscription.keys.auth) {
      throw new Error('Некорректный формат подписки');
    }
    
    return {
      endpoint: subscription.endpoint,
      keys: {
        p256dh: subscription.keys.p256dh,
        auth: subscription.keys.auth
      },
      deviceType: this.detectDeviceType()
    };
  }

  /**
   * Определяет тип устройства
   */
  private detectDeviceType(): string {
    const userAgent = navigator.userAgent || navigator.vendor || (window as any).opera || '';
    if (/android/i.test(userAgent)) {
      return 'Android';
    }
    if (/iPad|iPhone|iPod/.test(userAgent) && !(window as any).MSStream) {
      return 'iOS';
    }
    return 'Desktop';
  }

  /**
   * Сохраняет подписку на сервере
   */
  private async saveSubscription(subscription: PushSubscription): Promise<void> {
    try {
      const payload = {
        endpoint: subscription.endpoint,
        p256dh: subscription.keys.p256dh,
        auth: subscription.keys.auth,
        deviceType: subscription.deviceType || 'Unknown'
      };

      await api.post('/api/PushNotification/subscriptions', payload);
      console.log('Подписка успешно сохранена на сервере');
    } catch (error) {
      console.error('Ошибка при сохранении подписки на сервере:', error);
      throw error;
    }
  }

  /**
   * Удаляет подписку с сервера
   * Метод не должен использоваться в новой версии
   */
  private async deleteSubscription(endpoint: string): Promise<void> {
    console.log('Удаление подписки отключено в этой версии');
  }

  /**
   * Отправляет тестовое уведомление
   */
  async sendTestNotification(): Promise<boolean> {
    try {
      // Первоначально пробуем отправить через API
      try {
        await api.post('/api/PushNotification/test');
        return true;
      } catch (apiError) {
        console.warn('Не удалось отправить через API, используем локальное уведомление', apiError);
      }

      // Если API недоступен, отправляем локальное уведомление
      if ('Notification' in window && Notification.permission === 'granted') {
        // Пробуем через ServiceWorker
        if (this.swRegistration) {
          try {
            await this.swRegistration.showNotification('Тестовое уведомление', {
              body: 'Это тестовое уведомление от системы учета показаний',
              icon: '/logo192.png',
              badge: '/favicon.ico',
              data: {
                url: window.location.origin + '/user',
                dateOfArrival: Date.now(),
                primaryKey: 1
              }
            });
            return true;
          } catch (swError) {
            console.warn('Не удалось отправить через ServiceWorker:', swError);
          }
        }

        // Если ServiceWorker недоступен, используем обычное уведомление
        try {
          const notification = new Notification('Тестовое уведомление', {
            body: 'Это тестовое уведомление от системы учета показаний',
            icon: '/logo192.png'
          });
          notification.onclick = () => {
            window.focus();
            notification.close();
          };
          return true;
        } catch (notificationError) {
          console.error('Не удалось создать уведомление:', notificationError);
          return false;
        }
      }
      
      return false;
    } catch (error) {
      console.error('Ошибка при отправке тестового уведомления:', error);
      return false;
    }
  }

  /**
   * Инициализирует push-уведомления при запуске приложения
   * Включая обновление существующего сервис-воркера
   */
  async initializePushNotifications(): Promise<boolean> {
    if (!this.isSupported()) {
      console.warn('Push API не поддерживается в этом браузере');
      return false;
    }
    
    try {
      console.log('Инициализация push-уведомлений...');
      
      // Обновляем сервис-воркер, если он уже существует
      if ('serviceWorker' in navigator) {
        // Получаем все существующие регистрации
        const registrations = await navigator.serviceWorker.getRegistrations();
        
        if (registrations.length > 0) {
          console.log('Найдены существующие регистрации сервис-воркеров:', registrations.length);
          
          // Обновляем существующие регистрации
          for (const registration of registrations) {
            try {
              console.log('Проверяем обновления для сервис-воркера:', registration.scope);
              await registration.update();
              console.log('Сервис-воркер успешно обновлен');
            } catch (updateError) {
              console.warn('Не удалось обновить сервис-воркер:', updateError);
            }
          }
        } else {
          console.log('Сервис-воркеры не зарегистрированы, регистрируем новый');
          await this.registerServiceWorker();
        }
      }
      
      // Проверяем статус подписки после инициализации
      const status = await this.getSubscriptionStatus();
      console.log('Текущий статус подписки:', status);
      
      return status === 'subscribed';
    } catch (error) {
      console.error('Ошибка при инициализации push-уведомлений:', error);
      return false;
    }
  }
}

// Экспортируем одиночный экземпляр сервиса
export const pushNotificationService = new PushNotificationService();
export default pushNotificationService; 