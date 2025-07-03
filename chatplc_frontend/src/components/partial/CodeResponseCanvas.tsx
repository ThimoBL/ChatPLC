import {ApiResult} from "../../models/ApiResult";
import {Prism as SyntaxHighlighter} from "react-syntax-highlighter";
import {tomorrow} from "react-syntax-highlighter/dist/esm/styles/prism";
import React from "react";
import {Box, Button, CircularProgress, Typography} from "@mui/material";

interface CodeResponseCanvasProps {
    loading: boolean;
    isAccepting: boolean;
    codeResponse: ApiResult | null;
    onClickCodeAccept: (code: string) => void;
}

export default function CodeResponseCanvas({
                                               loading,
                                               isAccepting,
                                               codeResponse,
                                               onClickCodeAccept
                                           }: CodeResponseCanvasProps) {
    return (
        <>
            {loading ? (
                <Box sx={{display: 'flex'}}>
                    <CircularProgress/>
                    <Typography variant="body1" sx={{ml: 2}}>
                        Loading...
                    </Typography>
                </Box>
            ) : codeResponse && (
                <Box sx={{whiteSpace: "pre-wrap", p: 2, borderRadius: 2, bgcolor: "background.paper"}}>
                    {codeResponse.IsSuccess ? (
                        <>
                            <Button
                                variant="contained"
                                color="primary"
                                fullWidth
                                disabled={isAccepting}
                                onClick={
                                    () => onClickCodeAccept(codeResponse.Message?.content[0].text ?? "")
                                }
                                sx={{
                                    borderRadius: '25px'
                                }}
                            >
                                {
                                    isAccepting ? (
                                        <CircularProgress size={24} sx={{color: "white"}}/>
                                    ) : (
                                        "Accept Code"
                                    )
                                }
                            </Button>
                            <Box my={2}>
                                <SyntaxHighlighter
                                    language="pascal"
                                    style={tomorrow}
                                    customStyle={{borderRadius: "25px", padding: "1rem"}}
                                >
                                    {codeResponse?.Message?.content[0].text ?? ""}
                                </SyntaxHighlighter>
                            </Box>
                        </>
                    ) : (
                        <>
                            <Typography variant="body1" color="error">
                                An error occured:
                            </Typography>
                            <Typography variant="body1">
                                Type: {codeResponse?.Error?.error?.type ?? "Unkown type"}
                            </Typography>
                            <Typography variant="body1">
                                Message: {codeResponse?.Error?.error?.message ?? "Unkown message"}
                            </Typography>
                        </>
                    )}
                </Box>
            )}
        </>
    )
}