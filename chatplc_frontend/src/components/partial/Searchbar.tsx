import {Grid, IconButton, InputBase, Paper} from "@mui/material";
import SearchIcon from "@mui/icons-material/Search";

interface SearchbarProps {
    loading: boolean;
    question: string;
    setQuestion: (value: string) => void;
    onSubmit: () => void;
}

export default function Searchbar({loading, question, setQuestion, onSubmit}: SearchbarProps) {

    const handleSubmit = (event: any) => {
        event.preventDefault(); // Prevents page reload

        // Handle the submit logic of the upper layer
        onSubmit();
    };

    const handleKeyDown = (event: any) => {
        if (loading) return;

        // Submit on Enter without Shift
        if (event.key === 'Enter' && !event.shiftKey) {
            event.preventDefault(); // Prevents newline
            onSubmit();
        }
    };

    return (
        <Grid container spacing={3} sx={{p: 3}}>
            <Grid size="grow"></Grid>
            <Grid size={6}>
                <Paper
                    component="form"
                    elevation={9}
                    onSubmit={handleSubmit}
                    sx={{
                        p: '2px 4px',
                        display: 'flex',
                        alignItems: 'center',
                        borderRadius: '25px'
                    }}
                >
                    <InputBase
                        multiline
                        maxRows={5}
                        sx={{ml: 2, flex: 1}}
                        placeholder="Ask for a control module or a equipment module"
                        inputProps={{'aria-label': 'Ask for a control module or a equipment module'}}
                        value={question}
                        onChange={(e) => setQuestion(e.target.value)}
                        onKeyDown={handleKeyDown}
                        disabled={loading}
                    />
                    <IconButton type="submit" sx={{p: '10px'}} aria-label="search" disabled={loading}>
                        <SearchIcon/>
                    </IconButton>
                </Paper>
            </Grid>
            <Grid size="grow"></Grid>
        </Grid>
    )
}