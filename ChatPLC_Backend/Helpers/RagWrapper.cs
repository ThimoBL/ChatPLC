using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ChatPLC_Backend.Helpers;

public class RagWrapper : IRagWrapper
{
    private readonly ILogger<RagWrapper> _logger;
    private readonly HttpClient _httpClient;

    public RagWrapper(ILogger<RagWrapper> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    /// <summary>
    /// Ask a question to the RAG model, the model will return a json object with relevant code snippets
    /// </summary>
    /// <param name="question"></param>
    /// <returns></returns>
    public async Task<string> AskQuestion(string question)
    {
        try
        {
            _logger.LogInformation($"Asking question: {question}");
            
            // Create the content to send to the RAG model
            var query = JsonSerializer.Serialize(new { query = question });
            
            // Assuming the API expects a JSON object with a "question" field
            var content = new StringContent(query, Encoding.UTF8, "application/json");
            
            // Send the question to the RAG model
            var response = await _httpClient.PostAsync("test_embed_query", content);

            // Check if the response is successful
            if (response.IsSuccessStatusCode)
            {
                // Read the response content
                var result = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Response to question successfull");
                
                return result;
            }

            // Log the error if the response is not successful
            _logger.LogError($"Error: {response.StatusCode}");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while asking the question.");
            return null;
        }
    }
    
    public async Task<bool> SendFileToRagModel(string code)
    {
        try
        {
            _logger.LogInformation($"Sending code...");
            
            // Create the content to send to the RAG model
            var document = JsonSerializer.Serialize(new { document = code });
            
            // Assuming the API expects a JSON object with a "question" field
            var content = new StringContent(document, Encoding.UTF8, "application/json");
            
            // Send the question to the RAG model
            var response = await _httpClient.PostAsync("test_embed_document", content);

            if (response.IsSuccessStatusCode)
            {
                // Read the response content
                var result = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Response to code successfull: {result}");
                
                return true;
            }
            
            // Log the error if the response is not successful
            _logger.LogError($"Error: {response.StatusCode}");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while sending the file to the RAG model.");
            return false;
        }
    }
}