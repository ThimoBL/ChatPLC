import {Button} from "@mui/material";
import axios from "axios";
import {useState} from "react";
import {fetchTestEndpoint} from "../../api/ChatPLC_BackendAPI";
import {fetchIsUserLoggedIn} from "../../api/AuthService";

export default function TempPage() {
    const [testConnection, setTestConnection] = useState<string>("");
    const [data, setData] = useState<string>("");

    const test = async () => {
        try {
            const myHeaders = new Headers();
            myHeaders.append("X-CSRF", "1");

            let response = await axios.get("/test", {
                headers: {
                    "X-CSRF": 1,
                },
            });

            console.log(response.data);
            setTestConnection(response.data);
        } catch (e) {
            console.log(e)
        }
    }

    const getUserClaims = async () => {
        let response = await fetchIsUserLoggedIn();

        setData(JSON.stringify(response, null, 5));
    }

    return (
        <>
            <h1>Development Page</h1>
            <p>This is a page for developers only.</p>
            <Button onClick={test} variant="outlined"> Test </Button>
            <p>{testConnection}</p>
            <Button onClick={getUserClaims} variant="outlined"> Get User Claims </Button>
            <p><pre>{data}</pre></p>
        </>
    )
}