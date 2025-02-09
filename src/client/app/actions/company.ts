"use server";

import { apiClient } from "@/server/api-client";
import { Company } from "@/types/company";
import { revalidatePath } from "next/cache";

export async function updateCompany(formData: FormData) {
  const data = Object.fromEntries(formData.entries());

  await apiClient("/api/company", {
    method: "PUT",
    body: data,
  });

  revalidatePath("/settings");
}

export async function uploadLogo(formData: FormData) {
  await apiClient("/api/company/logo", {
    method: "POST",
    body: formData,
  });

  revalidatePath("/settings");
}

export async function removeLogo() {
  await apiClient("/api/company/logo", {
    method: "DELETE",
  });

  revalidatePath("/settings");
}

export async function getCompany() {
  return await apiClient<Company>("/api/company", {
    method: "GET",
  });
}
