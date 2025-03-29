import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  /* config options here */
  reactStrictMode: false,
  onError: () => {
    // Отключаем стандартный обработчик ошибок
  },
  onDemandEntries: {
    maxInactiveAge: 60 * 60 * 1000,
    pagesBufferLength: 5,
  },
  devIndicators: {
    buildActivity: true,
  },
};

export default nextConfig;
