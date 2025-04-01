import { Html, Head, Main, NextScript } from 'next/document';

export default function Document() {
  return (
    <Html lang="ru">
      <Head>
        {/* Мета-теги для мобильной версии */}
        <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no" />
        <meta name="theme-color" content="#3b82f6" />
        <meta name="description" content="Приложение для учета и контроля показаний счетчиков в квартирах" />
        
        {/* Манифест для PWA */}
        <link rel="manifest" href="/manifest.json" />
        
        {/* Иконки */}
        <link rel="icon" href="/favicon.ico" />
        <link rel="apple-touch-icon" href="/vercel.svg" />
      </Head>
      <body>
        <Main />
        <NextScript />
      </body>
    </Html>
  );
} 