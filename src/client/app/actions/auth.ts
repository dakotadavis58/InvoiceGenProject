"use server";

import { cookies } from "next/headers";
import { redirect } from "next/navigation";
import type { AuthResponse, RegisterRequest } from "@/types/auth";
import { apiClient } from "@/server/api-client";

interface FormState {
  error?: string;
}

export async function loginAction(
  state: FormState,
  formData: FormData
): Promise<FormState> {
  try {
    console.log("loginAction", formData, state);
    console.log("email", formData.get("email"));
    console.log("password", formData.get("password"));

    const response = await apiClient<AuthResponse>("/api/auth/login", {
      method: "POST",
      body: {
        email: formData.get("email"),
        password: formData.get("password"),
      },
    });

    console.log("response", response);

    const cookieStore = await cookies();
    cookieStore.set("token", response.token, {
      httpOnly: true,
      secure: process.env.NODE_ENV === "production",
      sameSite: "lax",
      path: "/",
    });

    // Don't wrap the redirect in try/catch
  } catch (err) {
    if (err instanceof Error && err.message.includes("NEXT_REDIRECT")) {
      // Let the redirect happen
      throw err;
    }
    return {
      error: err instanceof Error ? err.message : "Invalid email or password",
    };
  }

  // Move the redirect outside the try/catch
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
