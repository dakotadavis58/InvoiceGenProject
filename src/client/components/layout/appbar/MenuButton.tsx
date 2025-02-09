"use client";

interface MenuButtonProps {
  onToggle: () => void;
}

export const MenuButton = ({ onToggle }: MenuButtonProps) => (
  <button
    onClick={onToggle}
    className="p-2 rounded-full hover:bg-gray-100 dark:hover:bg-gray-800"
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
        d="M4 6h16M4 12h16M4 18h16"
      />
    </svg>
  </button>
);
