import {createContext, ReactNode, useContext, useEffect, useState} from "react";
import {Session} from "@toolpad/core";
import {fetchIsUserLoggedIn, UserClaims} from "../../api/AuthService";

interface AuthContextType {
    isAuthenticated: boolean;
    session: Session | null;
    signOutUrl: string;
}

const AuthContext = createContext<AuthContextType>({
    isAuthenticated: false,
    session: null,
    signOutUrl: "/bff/logout",
});

export default function AuthProvider({children}: { children: ReactNode }) {
    const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);
    const [session, setSession] = useState<Session | null>(null);
    const [signOutUrl, setSignOutUrl] = useState<string>("/bff/logout");

    useEffect(() => {
        fetchIsUserLoggedIn()
            .then((response: UserClaims[]) => {
                const logoutUrl = response.find(
                    (claim: UserClaims) => claim.type === "bff:logout_url"
                )?.value ?? signOutUrl;
                console.log(response)
                setSession({
                    user: {
                        id: response.find((claim: UserClaims) => claim.type === "sid")?.value,
                        name: response.find((claim: UserClaims) => claim.type === "name")?.value,
                        email: response.find((claim: UserClaims) => claim.type === "email")?.value,
                        image: null,
                    },
                });

                setIsAuthenticated(true);
                setSignOutUrl(logoutUrl);
            })
            .catch(error => {
                console.error("Error fetching user claims: ", error);
                setIsAuthenticated(false);
                setSession(null);
            })
    }, []);

    return (
        <AuthContext.Provider value={{ isAuthenticated, session, signOutUrl }}>
            {children}
        </AuthContext.Provider>
    );

}

export const useAuth = () => useContext(AuthContext);