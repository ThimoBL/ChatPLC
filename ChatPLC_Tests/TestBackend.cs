using ChatPLC_Backend.Helpers;
using ChatPLC_Backend.Models;
using ChatPLC_Backend.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ChatPLC_Tests
{
    /// <summary>
    /// Custom HttpMessageHandler that allows us to programmatically set the response
    /// which <see cref="HttpClient"/> will return. This avoids network calls during unit‑testing.
    /// </summary>
    internal sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> _handler;

        public StubHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
            => Task.FromResult(_handler(request, cancellationToken));
    }

    public class TestBackend
    {
        [Fact]
        public async Task GetResponseAsync_WithSuccessfulResponse_ReturnsSuccessResult()
        {
            // Arrange
            var apiMessage = new AnthropicApiMessage { Id = "msg_123", Role = "assistant", Type = "message" };
            var json = JsonSerializer.Serialize(apiMessage);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
                { Content = new StringContent(json, Encoding.UTF8, "application/json") };

            var httpHandler = new StubHttpMessageHandler((_, _) => response);
            var httpClient = new HttpClient(httpHandler) { BaseAddress = new Uri("https://api.anthropic.com/v1/") };
            var logger = Mock.Of<ILogger<AnthropicWrapper>>();

            var wrapper = new AnthropicWrapper(logger, httpClient);

            // Act
            var result = await wrapper.GetResponseAsync("Any question", new List<AnthropicApiExample>());

            // Assert – Success helper should wrap original message
            Assert.True(result.IsSuccess);
            Assert.Equal("msg_123", result.Message.Id);
        }

        [Fact]
        public async Task AskQuestion_WithSuccessStatus_ReturnsString()
        {
            // Arrange
            const string expected = "{\"snippets\":[\"code1\"]}";

            var httpHandler = new StubHttpMessageHandler((req, _) =>
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(expected, Encoding.UTF8, "application/json")
                });
            var httpClient = new HttpClient(httpHandler) { BaseAddress = new Uri("http://127.0.0.1:5000/") };
            var logger = Mock.Of<ILogger<RagWrapper>>();

            var wrapper = new RagWrapper(logger, httpClient);

            // Act
            var actual = await wrapper.AskQuestion("How to blink a LED?");

            // Assert
            Assert.Equal(expected, actual);
        }

        private static QuestionService CreateService(string ragResponseJson,
            AnthropicApiResult? anthropicResult = null, IEnumerable<string>? streamChunks = null)
        {
            var loggerMock = Mock.Of<ILogger<QuestionService>>();

            var ragMock = new Mock<IRagWrapper>();
            ragMock.Setup(r => r.AskQuestion(It.IsAny<string>())).ReturnsAsync(ragResponseJson);

            var anthropicMock = new Mock<IAnthropicWrapper>();

            if (anthropicResult is not null)
                anthropicMock.Setup(a =>
                        a.GetResponseAsync(
                            It.IsAny<string>(), 
                            It.IsAny<List<AnthropicApiExample>>(), 
                            It.IsAny<CancellationToken>()))
                    .ReturnsAsync(anthropicResult);

            if (streamChunks is not null)
            {
                anthropicMock.Setup(a =>
                        a.StreamResponseAsync(
                            It.IsAny<string>(),
                            It.IsAny<List<AnthropicApiExample>>(),
                            It.IsAny<CancellationToken>()))
                    .Returns((string q, IEnumerable<AnthropicApiExample> ex) => ToAsyncEnumerable(streamChunks));
            }

            return new QuestionService(loggerMock, ragMock.Object, anthropicMock.Object);
        }

        private static async IAsyncEnumerable<string> ToAsyncEnumerable(IEnumerable<string> chunks)
        {
            foreach (var chunk in chunks)
            {
                yield return chunk;
                await Task.Yield();
            }
        }

        private static readonly string _validRagJson = JsonSerializer.Serialize(new List<ScoredPoints>
        {
            new ScoredPoints
            {
                Payload = new Dictionary<string, string> { { "document", "some snippet" } },
                Score = 0.95
            }
        });

        [Fact]
        public async Task AskQuestionToStream_Throws_WhenRagReturnsNull()
        {
            var service = CreateService("   ");

            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await foreach (var _ in service.AskQuestionToStream("fail"))
                {
                }
            });
        }

        [Fact]
        public async Task AskQuestionToJson_ReturnsSerializedAnthropicResult()
        {
            // Arrange
            var anthropic = new AnthropicApiResult
            {
                Message = new AnthropicApiMessage { Id = "x", Role = "assistant", Type = "message" }
            };
            var service = CreateService(_validRagJson, anthropicResult: anthropic);

            // Act
            var json = await service.AskQuestionToJson("any");
            var back = JsonSerializer.Deserialize<AnthropicApiResult>(json);

            // Assert
            Assert.NotNull(back);
            Assert.True(back!.IsSuccess);
            Assert.Equal("x", back.Message.Id);
        }
    }
}