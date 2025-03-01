import type { Metadata } from "next";
import "../../styles/globals.css";
import { MainLayout } from "@/components/layout/MainLayout";
import { ThemeProvider } from "@/theme/ThemeContext";

export const metadata: Metadata = {
  title: "Create Next App",
  description: "Generated by create next app",
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <ThemeProvider>
      <MainLayout>{children}</MainLayout>
    </ThemeProvider>
  );
}
