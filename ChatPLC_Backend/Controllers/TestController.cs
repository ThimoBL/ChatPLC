using Microsoft.AspNetCore.Mvc;

namespace ChatPLC_Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly ILogger<QuestionController> _logger;

    public TestController(ILogger<QuestionController> logger)
    {
        _logger = logger;
    }
    
    [HttpGet]
    public IActionResult Test()
    {
        try
        {
            // Simulate some processing
            var result = "Test successful!";
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in TestController");
            return StatusCode(500, "Internal server error");
        }
    }
}