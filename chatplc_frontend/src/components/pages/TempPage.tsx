import {Button} from "@mui/material";
import axios from "axios";
import {useState} from "react";
import {fetchTestEndpoint} from "../../api/ChatPLC_BackendAPI";
import {fetchIsUserLoggedIn} from "../../api/AuthService";

export default function TempPage() {
    const [data, setData] = useState<string>("");
    let userClaims: any = null;

    function login() {
        window.location.href = "/bff/login";
    }

    function logout() {
        if (userClaims) {
            const logoutUrl = userClaims.find(
                (claim: any) => claim.type === "bff:logout_url"
            ).value;
            window.location.href = logoutUrl;
        } else {
            window.location.href = "/bff/logout";
        }
    }

    const test = async () => {
        // let result = await fetchTestEndpoint();
        //
        // if (result) {
        //     console.log("Response from test endpoint:", result);
        //     setData(result);
        // } else {
        //     console.error("Error fetching data");
        // }

        try {
            const myHeaders = new Headers();
            myHeaders.append("X-CSRF", "1");

            let response = await axios.get("/test", {
                headers: {
                    "X-CSRF": 1,
                },
            });

            console.log(response.data);
        } catch (e) {
            console.log(e)
        }
    }

    const getUserClaims = async () => {
        await fetchIsUserLoggedIn();
    }

    return (
        <>
            <h1>Temp Page</h1>
            <p>This is a temporary page.</p>
            <Button onClick={test} variant="outlined"> Test </Button>
            <p>{data}</p>
            <Button onClick={login} variant="outlined"> Login </Button>
            <Button onClick={logout} variant="outlined"> Logout </Button>
            <Button onClick={getUserClaims} variant="outlined"> Get User Claims </Button>
        </>
    )
}