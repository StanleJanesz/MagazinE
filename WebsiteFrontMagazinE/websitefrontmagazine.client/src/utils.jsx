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