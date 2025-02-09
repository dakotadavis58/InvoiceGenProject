import { NextRequest, NextResponse } from "next/server";
import { jwtDecode } from "jwt-decode";

function isTokenValid(token: string): boolean {
  try {
    const decoded = jwtDecode(token);
    if (!decoded || !decoded.exp) return false;

    // Get current time in seconds since epoch
    const now = Math.floor(Date.now() / 1000);
    console.log("now", now);
    console.log("decoded.exp", decoded.exp);

    console.log("decoded.exp > now", decoded.exp > now);
    // Compare directly with exp (both are now in seconds)
    return decoded.exp > now;
  } catch {
    return false;
  }
}

export function middleware(request: NextRequest) {
  // Skip middleware for server actions and API routes
  if (
    request.nextUrl.pathname.startsWith("/api") ||
    request.method === "POST"
  ) {
    return;
  }

  // Get the pathname
  const path = request.nextUrl.pathname;

  // Define public paths that don't require authentication
  const isPublicPath = path === "/login" || path === "/signup";

  // Get both tokens
  const refreshToken = request.cookies.get("refreshToken")?.value;
  const accessToken = request.cookies.get("token")?.value;

  console.log("accessToken", accessToken);
  console.log("refreshToken", refreshToken);

  // Check if either token is valid
  const isValid = {
    accessToken: accessToken && isTokenValid(accessToken),
    refreshToken: refreshToken ? true : false,
  };

  console.log("isvalid", isValid);

  // Redirect logic
  if (isPublicPath && isValid.accessToken && isValid.refreshToken) {
    return NextResponse.redirect(new URL("/dashboard", request.url));
  }

  if (!isPublicPath && (!isValid.accessToken || !isValid.refreshToken)) {
    return NextResponse.redirect(new URL("/login", request.url));
  }
}

// Configure which paths should trigger the middleware
export const config = {
  matcher: [
    /*
     * Match all request paths except:
     * - api routes (/api/*)
     * - _next/static (static files)
     * - _next/image (image optimization files)
     * - favicon.ico (favicon file)
     */
    "/((?!api|_next/static|_next/image|favicon.ico).*)",
  ],
};
