using System.Text.Json;
using ChatPLC_Backend.Helpers;
using ChatPLC_Backend.Models;
using ChatPLC_Backend.Services.Interfaces;

namespace ChatPLC_Backend.Services;

public class QuestionService : IQuestionService
{
    private readonly ILogger<QuestionService> _logger;
    private readonly IRagWrapper _ragWrapper;
    private readonly IAnthropicWrapper _anthropicWrapper;

    public QuestionService(ILogger<QuestionService> logger, IRagWrapper ragWrapper, IAnthropicWrapper anthropicWrapper)
    {
        _logger = logger;
        _ragWrapper = ragWrapper;
        _anthropicWrapper = anthropicWrapper;
    }

    public async IAsyncEnumerable<string> AskQuestionToStream(string question)
    {
        // Call the service to process the question
        var response = await _ragWrapper.AskQuestion(question);

        if (string.IsNullOrWhiteSpace(response))
            throw new Exception("No response from the service.");

        // Parse the response to ScoredPoints
        var scoredPoints = JsonSerializer.Deserialize<List<ScoredPoints>>(response);

        // Check if the response is null or empty
        if (scoredPoints == null || !scoredPoints.Any())
            _logger.LogWarning("No relevant code snippets found for question: {question}.", question);

        var examples = scoredPoints.Select(sp => new AnthropicApiExample
        {
            Question = question,
            IdealOutput = sp.Payload != null && sp.Payload.TryGetValue("document", out var document)
                ? document
                : string.Empty,
        }).ToList();

        // yield return the response from the Anthropic model
        await foreach (var chunk in _anthropicWrapper.StreamResponseAsync(question, examples))
        {
            yield return chunk;
        }
    }

    public async Task<string> AskQuestionToJson(string question)
    {
        try
        {
            // Call the service to process the question
            var response = await _ragWrapper.AskQuestion(question);

            if (string.IsNullOrWhiteSpace(response))
                throw new Exception("No response from the service.");

            // Parse the response to ScoredPoints
            var scoredPoints = JsonSerializer.Deserialize<List<ScoredPoints>>(response);

            // Check if the response is null or empty
            if (scoredPoints == null || !scoredPoints.Any())
                _logger.LogWarning("No relevant code snippets found for question: {question}.", question);

            //Convert scoredPoints to a list of AnthropicApiExample
            var examples = scoredPoints.Select(sp => new AnthropicApiExample
            {
                Question = question,
                IdealOutput = sp.Payload != null && sp.Payload.TryGetValue("document", out var document)
                    ? document
                    : string.Empty,
            }).ToList();

            // Call the Anthropic model with the question and examples and return the response
            var anthropicApiResult = await _anthropicWrapper.GetResponseAsync(question, examples);

            return JsonSerializer.Serialize(anthropicApiResult);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while asking question");
            throw;
        }
    }

    public async Task<bool> AskQuestion(string question)
    {
        try
        {
            // Call the service to process the question
            var response = await _ragWrapper.AskQuestion(question);

            if (string.IsNullOrWhiteSpace(response))
                throw new Exception("No response from the service.");

            // Parse the response to ScoredPoints
            var scoredPoints = JsonSerializer.Deserialize<List<ScoredPoints>>(response);

            // Check if the response is null or empty
            if (scoredPoints == null || !scoredPoints.Any())
                _logger.LogWarning("No relevant code snippets found for question: {question}.", question);

            return scoredPoints != null && scoredPoints.Any();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while asking question");
            throw;
        }
    }
}