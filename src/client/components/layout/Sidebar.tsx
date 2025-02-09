"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { navigationItems, NavItem } from "./Navigation";

interface SidebarProps {
  open: boolean;
  onClose: () => void;
}

export const Sidebar = ({ open, onClose }: SidebarProps) => {
  const pathname = usePathname();
  const [expandedItems, setExpandedItems] = useState<string[]>([]);
  const [isMobile, setIsMobile] = useState(false);

  useEffect(() => {
    const checkMobile = () => {
      setIsMobile(window.innerWidth < 768);
    };

    checkMobile();
    window.addEventListener("resize", checkMobile);
    return () => window.removeEventListener("resize", checkMobile);
  }, []);

  const handleExpandClick = (title: string) => {
    setExpandedItems((prev) =>
      prev.includes(title)
        ? prev.filter((item) => item !== title)
        : [...prev, title]
    );
  };

  const isCurrentPath = (path: string) => {
    return pathname === path || pathname?.startsWith(`${path}/`);
  };

  const renderNavItem = (item: NavItem, depth = 0) => {
    const isExpanded = expandedItems.includes(item.title);
    const isActive = isCurrentPath(item.path);
    const hasChildren = item.children && item.children.length > 0;

    return (
      <div key={item.title}>
        <div className={`pl-${depth * 4}`}>
          {hasChildren ? (
            <button
              onClick={() => handleExpandClick(item.title)}
              className={`
                w-full flex items-center px-3 py-2 text-sm rounded-lg
                ${
                  isActive
                    ? "text-primary-600 bg-primary-50 dark:bg-gray-800"
                    : "text-text-primary dark:text-dark-text-primary hover:bg-gray-100 dark:hover:bg-gray-800"
                }
                ${!open && "justify-center"}
              `}
              title={!open ? item.title : undefined}
            >
              <span className={open ? "mr-3" : ""}>
                <item.icon />
              </span>
              {open && (
                <>
                  <span className="flex-1">{item.title}</span>
                  <svg
                    className={`w-5 h-5 transform transition-transform ${
                      isExpanded ? "rotate-180" : ""
                    }`}
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      strokeWidth={2}
                      d="M19 9l-7 7-7-7"
                    />
                  </svg>
                </>
              )}
            </button>
          ) : (
            <Link
              href={item.path}
              onClick={() => isMobile && onClose()}
              className={`
                flex items-center px-3 py-2 text-sm rounded-lg
                ${
                  isActive
                    ? "text-primary-600 bg-primary-50 dark:bg-gray-800"
                    : "text-text-primary dark:text-dark-text-primary hover:bg-gray-100 dark:hover:bg-gray-800"
                }
                ${!open && "justify-center"}
              `}
              title={!open ? item.title : undefined}
            >
              <span className={open ? "mr-3" : ""}>
                <item.icon />
              </span>
              {open && <span>{item.title}</span>}
            </Link>
          )}
        </div>

        {/* Nested items - only show when sidebar is open */}
        {hasChildren && isExpanded && open && (
          <div className="mt-1">
            {item.children?.map((child) => renderNavItem(child, depth + 1))}
          </div>
        )}
      </div>
    );
  };

  return (
    <>
      {/* Backdrop for mobile */}
      {isMobile && open && (
        <div
          className="fixed inset-0 bg-black bg-opacity-50 z-drawer"
          onClick={onClose}
        />
      )}

      {/* Sidebar */}
      <aside
        className={`
          fixed top-header left-0 h-[calc(100vh-theme(spacing.header))]
          bg-white dark:bg-dark-background-paper
          border-r border-gray-200 dark:border-gray-800
          transition-all duration-300 ease-in-out z-drawer
          ${
            isMobile
              ? open
                ? "w-sidebar translate-x-0"
                : "-translate-x-full"
              : open
              ? "w-sidebar"
              : "w-16"
          }
        `}
      >
        {/* Mobile close button - only show on mobile */}
        {isMobile && (
          <button
            onClick={onClose}
            className="absolute top-2 right-2 p-2 rounded-full hover:bg-gray-100 dark:hover:bg-gray-800"
          >
            <svg
              className="w-6 h-6"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M6 18L18 6M6 6l12 12"
              />
            </svg>
          </button>
        )}

        {/* Navigation items */}
        <nav className="h-full overflow-y-auto py-4">
          <div className="space-y-1 px-3">
            {navigationItems.map((item) => renderNavItem(item))}
          </div>
        </nav>
      </aside>
    </>
  );
};
