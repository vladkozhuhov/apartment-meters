export interface PushSubscription {
  endpoint: string;
  keys: {
    p256dh: string;
    auth: string;
  };
}

export interface PushNotificationPayload {
  title: string;
  body: string;
  icon?: string;
  badge?: string;
  data?: Record<string, any>;
  tag?: string;
  renotify?: boolean;
  requireInteraction?: boolean;
  silent?: boolean;
  sound?: string;
  vibrate?: number[];
  image?: string;
  actions?: Array<{
    action: string;
    title: string;
    icon?: string;
  }>;
}

export interface PushNotificationSettings {
  userVisibleOnly: boolean;
  applicationServerKey: string | null;
}

export type NotificationStatus = 'default' | 'granted' | 'denied' | 'unsupported'; 