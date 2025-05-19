using System.Text.Json;
using ChatPLC_Backend.Helpers;
using ChatPLC_Backend.Models;
using ChatPLC_Backend.Models.RequestModels;
using ChatPLC_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ChatPLC_Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class QuestionController : ControllerBase
{
    private readonly ILogger<QuestionController> _logger;
    private readonly IQuestionService _questionService;

    public QuestionController(ILogger<QuestionController> logger, IQuestionService questionService)
    {
        _logger = logger;
        _questionService = questionService;
    }

    /// <summary>
    /// Streams the answer to the provided question back to the caller in chunks via SSE.
    /// </summary>
    /// <param name="request">Contains the question being asked.</param>
    [HttpPost("stream")]
    public async Task AskQuestionByStream([FromBody] QuestionRequest request)
    {
        try
        {
            var question = request.Question;

            if (string.IsNullOrWhiteSpace(question))
            {
                _logger.LogWarning("Empty question received in {MethodName}", nameof(AskQuestionByStream));
                Response.StatusCode = StatusCodes.Status400BadRequest;
                await Response.WriteAsync("Question cannot be empty.");
                return; // Make sure to exit the method after writing an error response
            }
            
            Response.Headers.Append("Content-Type", "text/event-stream");
            Response.Headers.Append("Cache-Control", "no-cache");
            Response.Headers.Append("Connection", "keep-alive");
            
            await foreach (var chunk in _questionService.AskQuestionToStream(question))
            {
                await Response.WriteAsync(chunk);
                await Response.Body.FlushAsync();
            }
            
            _logger.LogInformation("Streaming response completed for question: {Question}", question);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in {MethodName} for question: {Question}", nameof(AskQuestionByStream), request?.Question);
            Response.StatusCode = StatusCodes.Status400BadRequest;
            await Response.WriteAsync(ex.Message);
        }
    }

    /// <summary>
    /// Processes the question and returns a complete JSON response.
    /// </summary>
    /// <param name="request">Contains the question being asked.</param>
    [HttpPost("json")]
    public async Task<IActionResult> AskQuestionByJson([FromBody] QuestionRequest request)
    {
        try
        {
            var question = request.Question;

            if (string.IsNullOrWhiteSpace(question))
            {
                _logger.LogWarning("Empty question received in {MethodName}", nameof(AskQuestionByJson));
                return BadRequest("Question cannot be empty.");
            }
            
            _logger.LogInformation("AskQuestionByJson called for question: {Question}", question);
            
            var response = await _questionService.AskQuestionToJson(question);
            // var response = JsonSerializer.Serialize(SeedData.GetSeedData());
            if (string.IsNullOrWhiteSpace(response))
            {
                _logger.LogWarning("No response from service for question: {Question}", question);
                return NotFound("No response from the service.");
            }
            
            return Ok(response);
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in {MethodName} for question: {Question}", nameof(AskQuestionByJson), request?.Question);
            return BadRequest(ex.Message);
        }
    }
    
    /// <summary>
    /// Ask question from frontend and check relevant code fragments
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> AskQuestion([FromBody] QuestionRequest request)
    {
        try
        {
            var question = request.Question;
            
            if (string.IsNullOrWhiteSpace(question))
            {
                _logger.LogWarning("Empty question received in {MethodName}", nameof(AskQuestion));
                return BadRequest("Question cannot be empty.");
            }

            _logger.LogInformation("AskQuestion called for question: {Question}", question);

            // Call the service to process the question
            var response = await _questionService.AskQuestion(question);
            
            // if (string.IsNullOrWhiteSpace(response))
            //     return NotFound("No response from the service.");
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in {MethodName} for question: {Question}", nameof(AskQuestion), request?.Question);
            return BadRequest(ex.Message);
        }
    }
}