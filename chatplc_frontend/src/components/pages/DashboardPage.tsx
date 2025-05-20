import Searchbar from "../partial/Searchbar";
import {useState} from "react";
import {Alert, Grid} from "@mui/material";
import CodeResponseCanvas from "../partial/CodeResponseCanvas";
import {fetchPlcCodeJson} from "../../api/ChatPLC_BackendAPI";
import axios from "axios";
import {ApiResult} from "../../models/ApiResult";
import {useAuth} from "../providers/AuthProvider";

export default function DashboardPage() {
    const {isAuthenticated} = useAuth();

    const [loading, setLoading] = useState(false);
    const [question, setQuestion] = useState('');
    const [codeResponses, setCodeResponses] = useState<ApiResult[]>([]);

    const handleSubmit = async () => {
        console.log('Submitted query:', question);

        // Do whatever you want here (e.g., search, route, etc.)
        if (question) {
            // Set loading true
            setLoading(true);

            const requests = [
                await fetchPlcCodeJson(question),
                await fetchPlcCodeJson(question),
                await fetchPlcCodeJson(question)
            ];

            await axios.all(requests)
                .then((data) =>
                    setCodeResponses(data)
                )
                .catch((errors) =>
                    console.log(errors)
                ).finally(() => setLoading(false))

            setQuestion('')
        }
    };

    if (!isAuthenticated) {
        return (
            <div style={{height: '100%', width: '100%', display: 'flex', justifyContent: 'center', alignItems: 'center'}}>
                <Alert variant="outlined" severity="warning">You need to be logged in before you can use the question
                    feature</Alert>
            </div>
        );
    }

    return (
        <>
            <Searchbar
                loading={loading}
                question={question}
                setQuestion={setQuestion}
                onSubmit={handleSubmit}
            />
            <Grid container columnSpacing={{xs: 1, sm: 2, md: 3}} sx={{px: 3, height: '100%'}}>
                <Grid size={4} sx={{pr: 3, borderRight: '1px solid rgba(255, 255, 255, 0.12)'}}>
                    <CodeResponseCanvas loading={loading} codeResponse={codeResponses[0]}/>
                </Grid>
                <Grid size={4} sx={{pr: 3, borderRight: '1px solid rgba(255, 255, 255, 0.12)'}}>
                    <CodeResponseCanvas loading={loading} codeResponse={codeResponses[1]}/>
                </Grid>
                <Grid size={4} sx={{pr: 3}}>
                    <CodeResponseCanvas loading={loading} codeResponse={codeResponses[2]}/>
                </Grid>
            </Grid>
        </>
    )
}