"use server";

import { cookies } from "next/headers";
import { redirect } from "next/navigation";
import type { AuthResponse, RegisterRequest } from "@/types/auth";
import { apiClient } from "@/server/api-client";
import { headers } from "next/headers";

interface FormState {
  error?: string;
}

export async function loginAction(
  state: FormState,
  formData: FormData
): Promise<FormState> {
  try {
    console.log("Starting login action...");

    const response = await apiClient<AuthResponse>("/api/auth/login", {
      method: "POST",
      body: {
        email: formData.get("email"),
        password: formData.get("password"),
      },
      credentials: "include",
    });

    console.log("API Response:", response);

    // Set the cookies from the server action
    const cookieStore = await cookies();

    // Set access token cookie
    cookieStore.set("token", response.token, {
      httpOnly: true,
      secure: process.env.NODE_ENV === "production",
      sameSite: "lax",
      path: "/",
      maxAge: 15 * 60, // 15 minutes
    });

    // Set refresh token cookie
    cookieStore.set("refreshToken", response.refreshToken, {
      httpOnly: true,
      secure: process.env.NODE_ENV === "production",
      sameSite: "lax",
      path: "/",
      maxAge: 7 * 24 * 60 * 60, // 7 days
    });

    // Get the raw headers from the response
    const allCookies = cookieStore.getAll();
    console.log("All cookies:", allCookies);

    // Log individual cookies
    console.log("token cookie:", cookieStore.get("token"));
    console.log("refreshToken cookie:", cookieStore.get("refreshToken"));

    // Check if cookies are being set but not visible
    const hasToken = cookieStore.has("token");
    const hasRefreshToken = cookieStore.has("refreshToken");
    console.log(
      "Cookie existence check - token:",
      hasToken,
      "refreshToken:",
      hasRefreshToken
    );

    // Get the headers from the request object
    const requestHeaders = headers();
    console.log(
      "Request headers:",
      Object.fromEntries((await requestHeaders).entries())
    );
    console.log("Cookie header:", (await requestHeaders).get("cookie"));
  } catch (err) {
    if (err instanceof Error && err.message.includes("NEXT_REDIRECT")) {
      throw err;
    }
    return {
      error: err instanceof Error ? err.message : "Invalid email or password",
    };
  }

  redirect("/dashboard");
}

export async function signupAction(
  state: FormState,
  formData: FormData
): Promise<FormState> {
  try {
    const data: RegisterRequest = {
      email: formData.get("email") as string,
      password: formData.get("password") as string,
      firstName: formData.get("firstName") as string,
      lastName: formData.get("lastName") as string,
    };

    const response = await apiClient<AuthResponse>("/auth/register", {
      method: "POST",
      body: JSON.stringify(data),
    });

    const cookieStore = await cookies();
    cookieStore.set("token", response.token, {
      httpOnly: true,
      secure: process.env.NODE_ENV === "production",
      sameSite: "lax",
      path: "/",
    });

    redirect("/dashboard");
  } catch (err) {
    return {
      error: err instanceof Error ? err.message : "Registration failed",
    };
  }
}
