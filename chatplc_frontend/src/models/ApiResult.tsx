export interface ApiResult {
    Message: ApiMessage | null;
    Error: ApiError | null;
    IsSuccess: boolean;
}

export interface ApiMessage {
    id: string;
    type: string;
    role: string;
    model: string;
    content: ContentItem[];
    stop_reason: "end_turn" | "max_tokens" | "stop_sequence" | "tool_use";
    stop_sequence: string | null;
    usage: Usage;
}

export interface ContentItem {
    citations: object[] | null;
    text: string;
    type: string;
}

export interface Usage {
    input_tokens: number;
    cache_creation_input_tokens: number | null;
    cache_read_input_tokens: number | null;
    output_tokens: number;
}

export interface ApiError {
    type: string;
    error: {
        type: string;
        message: string;
    }
}

