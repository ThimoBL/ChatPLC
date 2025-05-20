import axios from "axios";

export interface UserClaims {
    type: string;
    value: string;
    valueType: string | null;
}

export async function fetchIsUserLoggedIn(): Promise<UserClaims[]> {
    try {
        const response = await axios.get("/bff/user", {
            headers: {
                "X-CSRF": 1,
            },
        });

        return response.data;
    } catch (error) {
        console.error(error);
        throw error;
    }
}