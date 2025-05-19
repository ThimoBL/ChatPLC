using ChatPLC_Backend.Models;

namespace ChatPLC_Backend;

public static class SeedData
{
    public static AnthropicApiResult GetSeedData()
    {
        return new AnthropicApiResult()
        {
            Message = new AnthropicApiMessage()
            {
                Id = "msg_01HFmAjYv1ZQFbLkm1SKXWEi",
                Type = "message",
                Role = "assistant",
                Model = "claude-3-7-sonnet-20250219",
                Content = new List<ContentItem>
                {
                    new ContentItem
                    {
                        Type = "text",
                        Text =
                            "<scl_file>\n// Automated valve control FB that manages valve operations in different modes (manual, automatic, simulation)\n// Controls valve open/close operations with feedback monitoring, alarm handling and HMI interface\n\nFUNCTION_BLOCK \\\"FB_AutomatedValve\\\"\n{ S7_Optimized_Access := 'TRUE' }\nVERSION : 0.1\n VAR_INPUT \n FB_Open : Bool; // Feedback signal indicating valve is open\n FB_Close : Bool; // Feedback signal indicating valve is closed\n Drive_OK : Bool; // Drive circuit status (1 = OK, 0 = Fault)\n END_VAR\n\n VAR_OUTPUT\n Cmd_open : Bool; // Command to open the valve\n END_VAR\n\n VAR_IN_OUT\n io_comm : \\\""
                    }
                },
                StopReason = "max_tokens",
                StopSequence = null,
                Usage = new Usage
                {
                    InputTokens = 2736,
                    CacheCreationInputTokens = 0,
                    CacheReadInputTokens = 0,
                    OutputTokens = 200
                }
            },
            Error = null
        };
    }
}