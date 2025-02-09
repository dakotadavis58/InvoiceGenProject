"use client";

import { useState } from "react";
import Link from "next/link";

export const NotificationsMenu = () => {
  const [open, setOpen] = useState(false);

  // Mock notifications - in real app, this would come from a state management system
  const notifications = [
    { id: 1, message: "Invoice #1234 has been paid", read: false },
    { id: 2, message: "New client registration", read: false },
    { id: 3, message: "Invoice #1235 is overdue", read: true },
  ];

  const unreadCount = notifications.filter((n) => !n.read).length;

  return (
    <div className="relative">
      <button
        onClick={() => setOpen(!open)}
        className="p-2 rounded-full hover:bg-gray-100 dark:hover:bg-gray-800"
      >
        <div className="relative">
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
              d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9"
            />
          </svg>
          {unreadCount > 0 && (
            <span className="absolute -top-1 -right-1 bg-red-500 text-white text-xs rounded-full h-5 w-5 flex items-center justify-center">
              {unreadCount}
            </span>
          )}
        </div>
      </button>

      {open && (
        <div className="absolute right-0 mt-2 w-80 bg-white dark:bg-dark-background-paper rounded-md shadow-lg ring-1 ring-black ring-opacity-5">
          <div className="py-1">
            {notifications.map((notification) => (
              <div
                key={notification.id}
                className={`px-4 py-2 hover:bg-gray-100 dark:hover:bg-gray-800 ${
                  !notification.read ? "border-l-4 border-primary-500" : ""
                }`}
              >
                <p
                  className={`text-sm ${
                    notification.read
                      ? "text-text-secondary dark:text-dark-text-secondary"
                      : "text-text-primary dark:text-dark-text-primary"
                  }`}
                >
                  {notification.message}
                </p>
              </div>
            ))}
            <div className="border-t border-gray-200 dark:border-gray-700">
              <Link
                href="/notifications"
                className="block px-4 py-2 text-sm text-center text-primary-500 hover:bg-gray-100 dark:hover:bg-gray-800"
              >
                View all notifications
              </Link>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
