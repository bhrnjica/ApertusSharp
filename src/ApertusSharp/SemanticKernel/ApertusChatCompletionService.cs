
using Microsoft.Extensions.AI;


namespace ApertusSharp.SemanticKernel
{
	/// <summary>
	/// Semantic Kernel compatible chat completion service wrapper for ApertusClient.
	/// This class provides a way to use ApertusClient in Semantic Kernel scenarios.
	/// </summary>
	public class ApertusChatCompletionService : IChatClient, IDisposable
	{
		private readonly IChatClient _chatClient;

		/// <summary>
		/// Initializes a new instance of the ApertusChatCompletionService.
		/// </summary>
		/// <param name="chatClient">The underlying chat client</param>
		public ApertusChatCompletionService(IChatClient chatClient)
		{
			_chatClient = chatClient ?? throw new ArgumentNullException(nameof(chatClient));
		}

		/// <summary>
		/// Initializes a new instance of the ApertusChatCompletionService with direct ApertusClient.
		/// </summary>
		/// <param name="apiKey">The API key</param>
		/// <param name="model">The model to use</param>
		/// <param name="endpoint">The endpoint URL</param>
		public ApertusChatCompletionService(string apiKey, string model = "swiss-ai/apertus-8b-instruct", string endpoint = "https://api.publicai.co")
		{
			_chatClient = new ApertusClient(model, apiKey, endpoint);
		}

		/// <inheritdoc/>
		public Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
		{
			return _chatClient.GetResponseAsync(messages, options, cancellationToken);
		}

		/// <inheritdoc/>
		public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
		{
			return _chatClient.GetStreamingResponseAsync(messages, options, cancellationToken);
		}

		/// <inheritdoc/>
		public object? GetService(Type serviceType, object? serviceKey = null)
		{
			return _chatClient.GetService(serviceType, serviceKey);
		}

		/// <inheritdoc/>
		public void Dispose()
		{
			if (_chatClient is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
	}
}