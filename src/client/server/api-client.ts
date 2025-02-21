import { cookies } from "next/headers";

const API_URL = process.env.API_URL || "http://localhost:4000";

interface RequestOptions extends RequestInit {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  body?: any;
  method?: string;
  headers?: Record<string, string>;
  parseResponse?: boolean;
  credentials?: RequestCredentials;
}

export async function apiClient<T>(
  endpoint: string,
  options: RequestOptions = {}
): Promise<T> {
  const url = `${API_URL}${endpoint}`;
  const cookieStore = await cookies();
  const token = cookieStore.get("token");

  const headers = new Headers({
    "Content-Type": "application/json",
    ...(options.headers as Record<string, string>),
  });

  if (token) {
    headers.set("Authorization", `Bearer ${token.value}`);
  }

  const response = await fetch(url, {
    ...options,
    headers,
    credentials: "include",
    body: options.body ? JSON.stringify(options.body) : undefined,
  });

  // Try to parse error responses regardless of parseResponse option
  if (!response.ok) {
    const errorText = await response.text();
    try {
      const errorJson = JSON.parse(errorText);
      throw new Error(
        errorJson.message || "An error occurred while fetching the data"
      );
    } catch (e: unknown) {
      // If error response isn't JSON, use the text directly
      console.error(e);
      throw new Error(errorText || "An error occurred while fetching the data");
    }
  }

  // For successful responses, only parse if parseResponse isn't false
  if (options.parseResponse !== false) {
    const text = await response.text();
    return text ? JSON.parse(text) : (undefined as T);
  }

  return undefined as T;
}
