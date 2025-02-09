import { createTheme, ThemeOptions } from "@mui/material/styles";
import { PaletteMode } from "@mui/material";

// Define custom colors and their variants
const getDesignTokens = (mode: PaletteMode): ThemeOptions => {
  return {
    palette: {
      mode,
      primary: {
        main: "#2563eb", // Blue-600
        light: "#60a5fa", // Blue-400
        dark: "#1e40af", // Blue-800
        contrastText: "#ffffff",
      },
      secondary: {
        main: "#059669", // Green-600
        light: "#34d399", // Green-400
        dark: "#047857", // Green-700
        contrastText: "#ffffff",
      },
      background: {
        default: mode === "light" ? "#f9fafb" : "#111827", // Gray-50 : Gray-900
        paper: mode === "light" ? "#ffffff" : "#1f2937", // White : Gray-800
      },
      text: {
        primary: mode === "light" ? "#111827" : "#f9fafb", // Gray-900 for light mode
        secondary: mode === "light" ? "#374151" : "#d1d5db", // Gray-700 for light mode
        disabled: mode === "light" ? "#6b7280" : "#9ca3af",
      },
      error: {
        main: "#dc2626", // Red-600
      },
      warning: {
        main: "#d97706", // Amber-600
      },
      info: {
        main: "#2563eb", // Blue-600
      },
      success: {
        main: "#059669", // Green-600
      },
      divider: mode === "light" ? "#e5e7eb" : "#374151", // Gray-200 : Gray-700
    },
    typography: {
      fontFamily: '"Inter", "Roboto", "Helvetica", "Arial", sans-serif',
      h1: {
        fontSize: "2.5rem",
        fontWeight: 600,
      },
      h2: {
        fontSize: "2rem",
        fontWeight: 600,
      },
      h3: {
        fontSize: "1.75rem",
        fontWeight: 600,
      },
      h4: {
        fontSize: "1.5rem",
        fontWeight: 500,
      },
      h5: {
        fontSize: "1.25rem",
        fontWeight: 500,
      },
      h6: {
        fontSize: "1rem",
        fontWeight: 500,
      },
      subtitle1: {
        fontSize: "1rem",
        fontWeight: 400,
      },
      subtitle2: {
        fontSize: "0.875rem",
        fontWeight: 500,
      },
      body1: {
        fontSize: "1rem",
      },
      body2: {
        fontSize: "0.875rem",
      },
    },
    shape: {
      borderRadius: 8,
    },
    components: {
      MuiButton: {
        styleOverrides: {
          root: {
            textTransform: "none",
            borderRadius: 8,
            padding: "8px 16px",
          },
          contained: {
            boxShadow: "none",
            "&:hover": {
              boxShadow: "none",
            },
          },
        },
      },
      MuiCard: {
        styleOverrides: {
          root: {
            borderRadius: 12,
            boxShadow:
              mode === "light"
                ? "0px 2px 4px -1px rgba(0,0,0,0.1), 0px 4px 5px 0px rgba(0,0,0,0.07), 0px 1px 10px 0px rgba(0,0,0,0.06)"
                : "0px 2px 4px -1px rgba(0,0,0,0.2), 0px 4px 5px 0px rgba(0,0,0,0.14), 0px 1px 10px 0px rgba(0,0,0,0.12)",
          },
        },
      },
      MuiPaper: {
        styleOverrides: {
          root: {
            backgroundImage: "none",
          },
        },
      },
    },
  };
};

// Create theme instance
export const createAppTheme = (mode: PaletteMode) => {
  const baseTheme = createTheme();
  const themeOptions = getDesignTokens(mode);

  return createTheme({
    ...themeOptions,
    shadows: baseTheme.shadows, // This ensures we have all 25 shadows
  });
};
