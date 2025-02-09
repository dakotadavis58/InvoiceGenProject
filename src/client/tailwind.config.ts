import type { Config } from "tailwindcss";

export default {
  content: [
    "./pages/**/*.{js,ts,jsx,tsx,mdx}",
    "./components/**/*.{js,ts,jsx,tsx,mdx}",
    "./app/**/*.{js,ts,jsx,tsx,mdx}",
  ],
  darkMode: "class",
  theme: {
    extend: {
      colors: {
        // Primary colors
        primary: {
          50: "#eff6ff",
          100: "#dbeafe",
          200: "#bfdbfe",
          300: "#93c5fd",
          400: "#60a5fa",
          500: "#3b82f6", // Main primary color
          600: "#2563eb",
          700: "#1d4ed8",
          800: "#1e40af",
          900: "#1e3a8a",
        },
        // Background colors
        background: {
          DEFAULT: "#f9fafb", // light mode background
          paper: "#ffffff", // light mode surface
        },
        // Text colors
        text: {
          primary: "#111827", // gray-900
          secondary: "#4b5563", // gray-600
          disabled: "#9ca3af", // gray-400
        },
        // Dark mode overrides
        dark: {
          background: {
            DEFAULT: "#111827", // gray-900
            paper: "#1f2937", // gray-800
          },
          text: {
            primary: "#f9fafb", // gray-50
            secondary: "#d1d5db", // gray-300
            disabled: "#6b7280", // gray-500
          },
        },
      },
      spacing: {
        sidebar: "240px",
        header: "64px",
      },
      zIndex: {
        drawer: "1200",
        appBar: "1300",
        modal: "1400",
        snackbar: "1500",
        tooltip: "1600",
      },
    },
  },
  plugins: [],
} satisfies Config;
