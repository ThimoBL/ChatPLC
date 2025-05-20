import {ApiResult} from "../../models/ApiResult";
import {Prism as SyntaxHighlighter} from "react-syntax-highlighter";
import {tomorrow} from "react-syntax-highlighter/dist/esm/styles/prism";
import React from "react";
import {Box, CircularProgress, Typography} from "@mui/material";

interface CodeResponseCanvasProps {
    loading: boolean;
    codeResponse: ApiResult | null;
}

export default function CodeResponseCanvas({loading, codeResponse}: CodeResponseCanvasProps) {
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
                        <Box my={2}>
                            <SyntaxHighlighter
                                language="pascal"
                                style={tomorrow}
                                customStyle={{borderRadius: "0.5rem", padding: "1rem"}}
                            >
                                {codeResponse?.Message?.content[0].text ?? ""}
                            </SyntaxHighlighter>
                        </Box>
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