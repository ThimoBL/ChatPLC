using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using ChatPLC_Backend.Models;

namespace ChatPLC_Backend.Helpers;

public class AnthropicWrapper : IAnthropicWrapper
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AnthropicWrapper> _logger;
    private readonly HttpClient _httpClient;

    private const int MaxTokens = 15000;

    private readonly Dictionary<string, string> _apiHeader;

    public AnthropicWrapper(ILogger<AnthropicWrapper> logger, HttpClient httpClient, IConfiguration configuration)
    {
        _logger = logger;
        _httpClient = httpClient;
        _configuration = configuration;
        
        // Initialize the API headers
        _apiHeader = new()
        {
            { "anthropic-version", "2023-06-01" },
            {
                "x-api-key",
                Environment.GetEnvironmentVariable("ASPNETCORE_ANTHROPIC_API_KEY") ?? ""
            }
        };
    }

    public async IAsyncEnumerable<string> StreamResponseAsync(string question, List<AnthropicApiExample>? examples,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // Create the content to send to the Anthropic model
        var requestBody = BuildRequestBody(
            useLatestModel: true,
            maxTokens: MaxTokens,
            question: question,
            stream: true,
            examples: examples);

        // Serialize the request body to JSON
        var document = JsonSerializer.Serialize(requestBody);

        // Create the HTTP request
        var request = new HttpRequestMessage(HttpMethod.Post, "messages")
        {
            Content = new StringContent(document, Encoding.UTF8, "application/json"),
        };

        // Set the headers for the request
        foreach (var header in _apiHeader)
        {
            request.Headers.Add(header.Key, header.Value);
        }

        // Send the request with streaming enabled
        HttpResponseMessage response;
        try
        {
            response = await _httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken
            );
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException httpEx)
        {
            _logger.LogError(httpEx, "An HTTP error occurred while sending the streaming request to Anthropic.");
            throw;
        }

        // Read the response content as a stream
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);

        var buffer = new StringBuilder();

        // Read the response line by line
        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            // Read the line of content
            var line = await reader.ReadLineAsync(cancellationToken);

            // Check if the line is empty
            if (string.IsNullOrWhiteSpace(line))
            {
                if (buffer.Length > 0)
                {
                    // If the buffer is not empty, yield the content
                    yield return buffer + "\n";
                    buffer.Clear();
                }
            }
            else
            {
                // Append the line to the buffer
                buffer.AppendLine(line);
            }
        }
    }

    public async Task<AnthropicApiResult> GetResponseAsync(string question, List<AnthropicApiExample>? examples,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Create the content to send to the Anthropic model
            var requestBody = BuildRequestBody(
                useLatestModel: true,
                maxTokens: MaxTokens,
                question: question,
                stream: false,
                examples: examples
            );

            // Serialize the request body to JSON
            var document = JsonSerializer.Serialize(requestBody);

            // Create the HTTP request
            var request = new HttpRequestMessage(HttpMethod.Post, "messages")
            {
                Content = new StringContent(document, Encoding.UTF8, "application/json"),
            };

            // Set the headers for the request
            foreach (var header in _apiHeader)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            // Send the request to the Anthropic model
            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            var result = await response.Content.ReadAsStringAsync();

            // Check if the response is successful
            if (response.IsSuccessStatusCode)
            {
                var message = JsonSerializer.Deserialize<AnthropicApiMessage>(result);
                
                // Response succeeded, log id
                _logger.LogInformation("Response successful, Anthropic response ID: {Id}", message?.Id);
                
                if (message != null)
                {
                    return AnthropicApiResult.Success(message);
                }
                _logger.LogError("Default response content has changed: {ResponseContent}", result);
                throw new Exception("Default response content has changed.");
            }

            // Deserialize the error response
            var errorMsg = JsonSerializer.Deserialize<AnthropicApiError>(result);
            if (errorMsg != null)
            {
                return AnthropicApiResult.Failure(errorMsg);
            }
            
            // Log error details if the response is not successful
            _logger.LogError(
                "Anthropic error. Status Code: {StatusCode}, Response Content: {ResponseContent}",
                response.StatusCode,
                result);

            throw new HttpRequestException(
                $"Error calling Anthropic API: {response.StatusCode}, Content: {result.ToString()}");
        }
        catch (HttpRequestException httpEx)
        {
            // Log and rethrow any HTTP-specific error for higher-level handling
            _logger.LogError(httpEx, "An HTTP error occurred while sending the request to Anthropic.");
            throw;
        }
        catch (Exception ex)
        {
            // Catch-all for any other exceptions
            _logger.LogError(ex, "An error occurred while processing the Anthropic response.");
            throw;
        }
    }

    private static object BuildRequestBody(bool useLatestModel, int maxTokens, string question, bool stream,
        List<AnthropicApiExample>? examples)
    {
        List<object> content = [];

        // Create the content to send to the Anthropic model
        var model = useLatestModel ? "claude-3-7-sonnet-latest" : "claude-3-5-sonnet-latest";

        // build the content for request body
        foreach (var example in examples)
        {
            var context = example.AdditionalContext != null
                ? $"\n<example_description>\n{example.AdditionalContext}\n</example_description>"
                : null;

            content.Add(new
            {
                type = "text",
                text =
                    $"<examples>\n<example>{context}\n<question>\n{example.Question}\n</question>\n<ideal_output>\n{example.IdealOutput}\n</ideal_output>\n</example>\n</examples>\n\n"
            });
        }

        // Add the user question to the content
        var prompt =
            $"You are an expert Siemens PLC programmer specializing in writing clean, maintainable, and professional Structured Control Language (SCL) code. Your task is to write new SCL code that solves a user's request, potentially using provided code fragments as inspiration. The code fragments serve as reference material only, and should not dictate the structure or implementation. Prioritize answering the question clearly, and use the fragments as supporting inspiration if helpful.\n\nHere is the user's question:\n<user_question>\n{question}\n</user_question>\n\nHere are up to three relevant code fragments that may serve as reference material split with <snippet></snippet>:\n<code_fragments>\n-\n</code_fragments>\n\nFollow these guidelines when writing your SCL file to complete the task:\n\n1. Carefully read and analyze the user's question. This should be your primary focus when creating the solution.\n\n2. If code fragments are provided, review them briefly, but do not copy them unless they perfectly match the required logic for the user's question.\n\n3. Start with two lines of comments at the very top summarizing what the code snippet does and with what hardware it interacts.\n\n4. Write new SCL code that directly addresses the user's question. Ensure your code is:\n   - Maintainable\n   - Professional\n   - Follows Siemens SCL best practices\n\n\n5. Write clean and structured code using regions, such as:\n      REGION VARIABLES\n      REGION CONDITIONS FOR TRANSITIONS\n      REGION COORDINATION CONTROL\n      REGION ALARMS\n\n6. Add a comment above each region explaining its role in the context of the full system.\n\n7. Use meaningful variable names and follow consistent naming conventions.\n\n8. Include inline comments to explain important logic or complex parts of your code. Comments should be concise and provide valuable insights.\n\n9. Structure your code with proper indentation and spacing for readability.\n\n10. Write clean, production-ready code that can be handed off to other engineers. Prioritize maintainability and readability.\n\n11. Double-check that your solution fully addresses all aspects of the user's question.\n\n12. Output only the new SCL code with comments. Do not include any explanations or discussions outside of the code and its comments.\n\nYour output should be a complete SCL file that addresses the user's question. Begin your response with <scl_file> and end it with </scl_file>. Do not include any explanation or commentary outside of these tags - your entire response should be the SCL file itself, including comments as specified in the guidelines above.";

        // Add the user prompt to the content
        content.Add(new
        {
            type = "text",
            text = prompt
        });

        return new
        {
            model,
            max_tokens = maxTokens,
            temperature = 1,
            system =
                "You are an expert Siemens PLC programmer specializing in writing clean, maintainable, and professional Structured Control Language (SCL) code. ",
            stream,
            messages = new[]
            {
                new
                {
                    role = "user", content
                }
            }
        };
    }
}