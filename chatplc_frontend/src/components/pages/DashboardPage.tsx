import Searchbar from "../partial/Searchbar";
import {useState} from "react";
import {Alert, Button, Grid} from "@mui/material";
import CodeResponseCanvas from "../partial/CodeResponseCanvas";
import {AcceptCode, fetchPlcCodeJson} from "../../api/ChatPLC_BackendAPI";
import axios from "axios";
import {ApiResult} from "../../models/ApiResult";
import {useAuth} from "../providers/AuthProvider";
import {toast, ToastContainer} from "react-toastify";

export default function DashboardPage() {
    const {isAuthenticated} = useAuth();

    const [isAccepting, setIsAccepting] = useState(false);
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
                fetchPlcCodeJson(question),
                fetchPlcCodeJson(question),
                fetchPlcCodeJson(question)
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
            <div style={{
                height: '100%',
                width: '100%',
                display: 'flex',
                justifyContent: 'center',
                alignItems: 'center'
            }}>
                <Alert variant="outlined" severity="warning">You need to be logged in before you can use the question
                    feature</Alert>
            </div>
        );
    }

    const onClickCodeAccept = async (code: string) => {
        setIsAccepting(true);

        AcceptCode(code)
            .then((response) => {
                console.log('Code accepted:', response);
                toast.success('Code accepted successfully!');
            })
            .catch((error) => {
                console.error('Error accepting code:', error);
                toast.error('Error accepting code: ' + (error instanceof Error ? error.message : 'Unknown error'));
            })
            .finally(() => {
                setIsAccepting(false);
                setQuestion('')
                setCodeResponses([]);
            });
    }

    return (
        <>
            <ToastContainer
                autoClose={10000}
                closeOnClick={true}
                theme="colored"
            />
            <Grid container spacing={3} sx={{p: 3}}>
                <Grid size="grow">
                </Grid>
                <Grid size={6}>
                    <Searchbar
                        loading={loading}
                        isAccepting={isAccepting}
                        question={question}
                        setQuestion={setQuestion}
                        onSubmit={handleSubmit}
                    />
                </Grid>
                <Grid size="grow">
                    {codeResponses.length > 0 && (
                        <Button
                            fullWidth
                            color="primary"
                            variant="contained"
                            disabled={isAccepting}
                            sx={{borderRadius: '25px', height: '100%'}}
                        >
                            Upload own code
                        </Button>
                    )}
                </Grid>
            </Grid>
            <Grid container columnSpacing={{xs: 1, sm: 2, md: 3}} sx={{px: 3, height: '100%'}}>
                <Grid size={4} sx={{pr: 3, borderRight: '1px solid rgba(255, 255, 255, 0.12)'}}>
                    <CodeResponseCanvas
                        loading={loading}
                        isAccepting={isAccepting}
                        codeResponse={codeResponses[0]}
                        onClickCodeAccept={onClickCodeAccept}/>
                </Grid>
                <Grid size={4} sx={{pr: 3, borderRight: '1px solid rgba(255, 255, 255, 0.12)'}}>
                    <CodeResponseCanvas
                        loading={loading}
                        isAccepting={isAccepting}
                        codeResponse={codeResponses[1]}
                        onClickCodeAccept={onClickCodeAccept}/>
                </Grid>
                <Grid size={4} sx={{pr: 3}}>
                    <CodeResponseCanvas
                        loading={loading}
                        isAccepting={isAccepting}
                        codeResponse={codeResponses[2]}
                        onClickCodeAccept={onClickCodeAccept}/>
                </Grid>
            </Grid>
        </>
    )
}