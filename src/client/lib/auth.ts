"use client";

import { logout as serverLogout } from "@/server/auth";

export const getToken = () => {
  return localStorage.getItem("token");
};

export const setToken = (token: string) => {
  localStorage.setItem("token", token);
  // The server should set the HTTP-only cookie during login/refresh
};

export const removeToken = () => {
  localStorage.removeItem("token");
};

// ... existing auth.ts code ...

export async function handleLogout() {
  await serverLogout();
  window.location.href = "/login";
}
