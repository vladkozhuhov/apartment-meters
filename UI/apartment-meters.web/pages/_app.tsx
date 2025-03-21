import React from 'react';
import dynamic from 'next/dynamic';
import '../styles/globals.css';
import type { AppProps } from 'next/app';

// Используем динамический импорт для Layout с отключенным SSR
const Layout = dynamic(() => import('./layout'), { ssr: false });

function MyApp({ Component, pageProps }: AppProps) {
  return (
    <Layout>
      <Component {...pageProps} />
    </Layout>
  );
}

export default MyApp;