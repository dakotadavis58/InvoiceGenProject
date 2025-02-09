"use server";

import { cookies } from "next/headers";

// Server-side auth utilities
export async function getServerSession() {
  const cookieStore = await cookies();
  const token = cookieStore.get("token");

  if (!token) return null;

  try {
    // Call your API to validate the session
    const response = await fetch(`${process.env.API_URL}/auth/validate`, {
      headers: {
        Cookie: `token=${token.value}`,
      },
    });

    if (!response.ok) return null;

    const data = await response.json();
    return data;
  } catch (error) {
    console.error("Session validation error:", error);
    return null;
  }
}

// Server Actions
export async function login(formData: FormData) {
  const email = formData.get("email");
  const password = formData.get("password");

  try {
    const response = await fetch(`${process.env.API_URL}/auth/login`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ email, password }),
    });

    if (!response.ok) {
      throw new Error("Login failed");
    }

    const data = await response.json();

    // Set the HTTP-only cookie
    (await cookies()).set("token", data.token, {
      httpOnly: true,
      secure: process.env.NODE_ENV === "production",
      sameSite: "lax",
      path: "/",
    });

    return { success: true };
  } catch (error) {
    console.error("Login error:", error);
    return { success: false, error: "Login failed" };
  }
}

export async function logout() {
  try {
    // Call logout endpoint if needed
    await fetch(`${process.env.API_URL}/auth/logout`, {
      method: "POST",
    });
  } catch (error) {
    console.error("Logout error:", error);
  }

  // Always clear the cookie
  (await cookies()).delete("token");

  return { success: true };
}
