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

// Helper function for startCheckout
export async function GetUserId(token) {
    try {
        const response = await fetch('https://localhost:8083/PersonalInfo', {
            headers: {
                Authorization: `Bearer ${token}`,
            }
        });

        if (!response.ok) {
            throw new Error(`Failed to fetch: ${await response.text()}`);
        }
        const data = await response.json();

        console.log(data);
        return data.id;

    }
    catch (error) {
        console.error(error);
    }

}

// Function that redirects to checkout for subcription
export async function startCheckout() {
    try {
        const token = getTokenFromCookie();
        const userId = await GetUserId(token);
        const res = await fetch(`https://localhost:8083/subscriptions/${userId}`, {
            method: 'POST',
            headers: {
                Authorization: `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });

        if (!res.ok) {
            const errorText = await res.text();
            console.error('Server error:', errorText);
            throw new Error(`HTTP error! status: ${res.status}`);
        }

        const data = await res.json();
        const { sessionId } = data;
        const stripe = await stripePromise;
        await stripe?.redirectToCheckout({ sessionId });
    } catch (error) {
        console.error('Checkout error:', error);
    }
}
