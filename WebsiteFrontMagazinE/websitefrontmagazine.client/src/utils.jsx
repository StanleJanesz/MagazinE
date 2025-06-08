// File with useful functions

// Function to retrieve jwt token from cookies
export function getTokenFromCookie() {
    const cookieName = "jwt=";
    const cookies = document.cookie.split("; ");

    for (const cookie of cookies) {
        if (cookie.startsWith(cookieName)) {
            return cookie.substring(cookieName.length);
        }
    }

    return null; 
};

// Function to save jwt token to cookies
export function saveTokenToCookie(token) {
    const expirationDays = 7;
    const expires = new Date(Date.now() + expirationDays * 24 * 60 * 60 * 1000).toUTCString();
    document.cookie = `jwt=${token}; path=/; expires=${expires}; secure`;
};
