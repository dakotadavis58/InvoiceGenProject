"use server";

import { apiClient } from "@/server/api-client";

interface FormState {
  error?: string;
  success?: boolean;
}

export async function changePasswordAction(
  state: FormState,
  formData: FormData
): Promise<FormState> {
  try {
    const currentPassword = formData.get("currentPassword") as string;
    const newPassword = formData.get("newPassword") as string;
    const confirmPassword = formData.get("confirmPassword") as string;

    if (newPassword !== confirmPassword) {
      return { error: "Passwords do not match" };
    }

    await apiClient("/api/auth/change-password", {
      method: "POST",
      body: {
        currentPassword,
        newPassword,
      },
      parseResponse: false,
    });

    return { success: true };
  } catch (error) {
    return {
      error:
        error instanceof Error ? error.message : "Failed to change password",
    };
  }
}
