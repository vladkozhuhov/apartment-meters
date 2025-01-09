// import Link from 'next/link';
import React, { ReactNode } from 'react';

interface LayoutProps {
  children: ReactNode;
}

const Layout: React.FC<LayoutProps> = ({ children }) => {
    return (
      <div className="flex flex-col min-h-screen">
        <header className="bg-blue-600 text-white py-4 px-8">
          <h1 className="text-2xl font-bold">
            
          </h1>
        </header>
        <main className="flex-grow container mx-auto px-4 py-6">
          {children}
        </main>
        <footer className="bg-gray-800 text-white py-4 px-8 text-center">
          <p>&copy; {new Date().getFullYear()} My Application. All rights reserved.</p>
        </footer>
      </div>
    );
  };
  
  export default Layout;