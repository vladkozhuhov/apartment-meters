import { useState, useEffect, useCallback } from 'react';
import pushNotificationService from '../services/pushNotificationService';

export type NotificationStatus = 'loading' | 'subscribed' | 'not-subscribed' | 'not-supported';

/**
 * Хук для работы с push-уведомлениями
 */
export const usePushNotification = () => {
  const [status, setStatus] = useState<NotificationStatus>('loading');
  const [isProcessing, setIsProcessing] = useState(false);
  
  // Базовая проверка поддержки
  const isSupported = pushNotificationService.isSupported();
  
  // Проверка статуса подписки
  const checkSubscription = useCallback(async () => {
    if (!isSupported) {
      setStatus('not-supported');
      return 'not-supported';
    }
    
    try {
      const subscriptionStatus = await pushNotificationService.getSubscriptionStatus();
      setStatus(subscriptionStatus);
      return subscriptionStatus;
    } catch (error) {
      console.error('Ошибка проверки статуса:', error);
      setStatus('not-subscribed');
      return 'not-subscribed';
    }
  }, [isSupported]);
  
  // Подписка на уведомления
  const autoSubscribe = useCallback(async () => {
    if (isProcessing || !isSupported) return null;
    
    setIsProcessing(true);
    
    try {
      const subscription = await pushNotificationService.subscribe();
      
      if (subscription) {
        setStatus('subscribed');
      } else {
        setStatus('not-subscribed');
      }
      
      return subscription;
    } catch (error) {
      console.error('Ошибка подписки:', error);
      setStatus('not-subscribed');
      return null;
    } finally {
      setIsProcessing(false);
    }
  }, [isProcessing, isSupported]);
  
  // Отправка тестового уведомления
  const sendTestNotification = useCallback(async () => {
    if (isProcessing) return false;
    
    setIsProcessing(true);
    
    try {
      return await pushNotificationService.sendTestNotification();
    } catch (error) {
      console.error('Ошибка отправки уведомления:', error);
      return false;
    } finally {
      setIsProcessing(false);
    }
  }, [isProcessing]);
  
  // Инициализация
  useEffect(() => {
    let isMounted = true;
    
    if (!isSupported) {
      setStatus('not-supported');
      return;
    }
    
    const init = async () => {
      try {
        await checkSubscription();
      } catch (error) {
        if (isMounted) setStatus('not-subscribed');
      }
    };
    
    init();
    
    return () => { isMounted = false; };
  }, [isSupported, checkSubscription]);
  
  return {
    status,
    isProcessing,
    isSupported,
    sendTestNotification,
    checkSubscription,
    autoSubscribe,
    // Для совместимости с существующим кодом
    subscribe: autoSubscribe,
    unsubscribe: () => Promise.resolve(false)
  };
};

export default usePushNotification; 