import {IconButton, InputBase, Paper} from "@mui/material";
import SearchIcon from "@mui/icons-material/Search";

interface SearchbarProps {
    loading: boolean;
    isAccepting: boolean;
    question: string;
    setQuestion: (value: string) => void;
    onSubmit: () => void;
}

export default function Searchbar({loading, isAccepting, question, setQuestion, onSubmit}: SearchbarProps) {

    const handleSubmit = (event: any) => {
        event.preventDefault(); // Prevents page reload

        // Handle the submit logic of the upper layer
        onSubmit();
    };

    const handleKeyDown = (event: any) => {
        if (loading || isAccepting) return;

        // Submit on Enter without Shift
        if (event.key === 'Enter' && !event.shiftKey) {
            event.preventDefault(); // Prevents newline
            onSubmit();
        }
    };

    return (
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
                disabled={loading || isAccepting}
            />
            <IconButton type="submit" sx={{p: '10px'}} aria-label="search" disabled={loading || isAccepting}>
                <SearchIcon/>
            </IconButton>
        </Paper>
    )
}