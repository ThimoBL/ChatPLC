using System.Runtime.CompilerServices;
using ChatPLC_Backend.Models;

namespace ChatPLC_Backend.Helpers;

public interface IAnthropicWrapper
{
    IAsyncEnumerable<string> StreamResponseAsync(string question, List<AnthropicApiExample>? examples,
        CancellationToken cancellationToken = default);

    Task<AnthropicApiResult> GetResponseAsync(string question, List<AnthropicApiExample>? examples,
        CancellationToken cancellationToken = default);
}