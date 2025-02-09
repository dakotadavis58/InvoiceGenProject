"use client";

import { useState, useEffect } from "react";
import { AppBar } from "./AppBar";
import { Sidebar } from "./Sidebar";

interface MainLayoutProps {
  children: React.ReactNode;
}

export const MainLayout = ({ children }: MainLayoutProps) => {
  const [mounted, setMounted] = useState(false);
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const [isMobile, setIsMobile] = useState(false);

  useEffect(() => {
    setMounted(true);
    const checkMobile = () => {
      const isMobileView = window.innerWidth < 768;
      setIsMobile(isMobileView);
      setSidebarOpen(!isMobileView);
    };

    checkMobile();
    window.addEventListener("resize", checkMobile);
    return () => window.removeEventListener("resize", checkMobile);
  }, []);

  const handleSidebarToggle = () => {
    setSidebarOpen(!sidebarOpen);
  };

  // Server-side and initial client render
  if (!mounted) {
    return (
      <div className="flex min-h-screen">
        {/* Skeleton AppBar */}
        <div className="fixed top-0 left-0 right-0 h-header z-appBar bg-background-paper dark:bg-dark-background-paper border-b border-gray-200 dark:border-gray-800" />

        {/* Skeleton Sidebar */}
        <div className="fixed top-header left-0 h-[calc(100vh-theme(spacing.header))] w-sidebar bg-background-paper dark:bg-dark-background-paper border-r border-gray-200 dark:border-gray-800" />

        {/* Main content with matching margin */}
        <main className="flex-grow p-3 mt-16 ml-sidebar bg-white dark:bg-gray-900">
          {children}
        </main>
      </div>
    );
  }

  return (
    <div className="flex min-h-screen">
      <AppBar onSidebarToggle={handleSidebarToggle} />
      <Sidebar open={sidebarOpen} onClose={() => setSidebarOpen(false)} />
      <main
        className={`
          flex-grow p-3 mt-16 
          bg-white dark:bg-gray-900
          transition-[margin] duration-300 ease-in-out
          ${isMobile ? "ml-0" : sidebarOpen ? "ml-sidebar" : "ml-16"}
        `}
      >
        {children}
      </main>
    </div>
  );
};
