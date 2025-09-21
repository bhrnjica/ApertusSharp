// Ignore Spelling: Apertus

using ApertusSharp;
using ApertusSharp.Entities;
using Microsoft.Extensions.AI;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

public class ApertusClient : IChatClient, IApertusApiClient, IDisposable
{
	private readonly HttpClient _httpClient;
	private readonly string _apiKey;
	private readonly string _model;
	private readonly string _baseEndpoint;
	private readonly string _apiVersion = "/v1";
	private readonly string _chatEndpoint = "/chat/completions";
	private readonly string _modelsEndpoint = "/models";

	public ApertusClient(string model, string apiKey, string endpoint = "https://api.publicai.co", HttpClient? httpClient = null)
	{
		_apiKey = apiKey;
		_model = model;
		_baseEndpoint = endpoint;
		_chatEndpoint = endpoint + _apiVersion + _chatEndpoint;
		_modelsEndpoint = endpoint + _apiVersion + _modelsEndpoint;
		_httpClient = httpClient = new HttpClient();

		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
		_httpClient.DefaultRequestHeaders.Add("User-Agent", "ApertusSharp/1.0");
	}

	public ApertusClient(string apiKey, string endpoint = "https://api.publicai.co", HttpClient? httpClient = null)
		: this(string.Empty, apiKey, endpoint, httpClient)
	{
	}

	/// <summary>
	/// Equivalent to:
	/// 
	/// curl --request POST \
	///     --url https://api.publicai.co/v1/chat/completions \
	///     --header 'Authorization: <string>' \
	///     --header 'Content-Type: application/json' \
	///     --header 'User-Agent: <string>' \
	///     --data '
	///   {
	///     "model": "swiss-ai/apertus-8b-instruct",
	///     "messages": [
	///       {
	///         "role": "user",
	///         "content": "Hello!"
	///   
	///   	}
	///     ]
	///   }
	///   '
	/// </summary>
	/// <param name="messages"></param>
	/// <param name="options"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	public async Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
	{
		var payload = new
		{
			model = _model,
			messages = messages.Select(m => new { role = m.Role.ToString().ToLower(), content = m.Text }),
			stream = false
		};

		var request = CreateChatRequest(payload);

		using var response = await _httpClient.SendAsync(request, cancellationToken);
		response.EnsureSuccessStatusCode();

		var json = await response.Content.ReadAsStringAsync(cancellationToken);
		using var doc = JsonDocument.Parse(json);
		var root = doc.RootElement;

		var content = root
			.GetProperty("choices")[0]
			.GetProperty("message")
			.GetProperty("content")
			.GetString() ?? string.Empty;

		return new ChatResponse(new ChatMessage(ChatRole.Assistant, content));
	}

	/// <summary>
	/// Simple string-based chat completion for convenience.
	/// </summary>
	/// <param name="prompt">The user prompt</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The assistant's response as a string</returns>
	public async Task<string> GetResponseAsync(string prompt, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(prompt))
			throw new ArgumentException("Prompt cannot be null or empty.", nameof(prompt));

		var messages = new[] { new ChatMessage(ChatRole.User, prompt) };
		var response = await GetResponseAsync(messages, null, cancellationToken);
		return response.Messages.FirstOrDefault()?.Text ?? string.Empty;
	}

	/// <summary>
	/// Equivalent to:
	/// 
	/// curl --request POST \
	///     --url https://api.publicai.co/v1/chat/completions \
	///     --header 'Authorization: <string>' \
	///     --header 'Content-Type: application/json' \
	///     --header 'User-Agent: <string>' \
	///     --data '
	///   {
	///     "model": "swiss-ai/apertus-8b-instruct",
	///     "messages": [
	///       {
	///         "role": "user",
	///         "content": "Hello!"
	///   
	///   	}
	///     ]
	///   }
	///   '
	/// </summary>
	/// <param name="messages"></param>
	/// <param name="options"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var payload = new
		{
			model = _model,
			messages = messages.Select(m => new { role = m.Role.ToString().ToLower(), content = m.Text }),
			stream = true
		};

		var request = CreateChatRequest(payload);

		using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
		response.EnsureSuccessStatusCode();

		using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
		await foreach (var update in ReadStreamingResponse(stream, cancellationToken))
		{
			yield return update;
		}
	}
	public async IAsyncEnumerable<ChatResponseUpdate> GenerateAsync(string prompt, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(prompt))
			throw new ArgumentException("Prompt cannot be null or empty.", nameof(prompt));

		var payload = new
		{
			model = _model,
			messages = new[] { new { role = "user", content = prompt } },
			stream = true
		};

		var request = CreateChatRequest(payload);

		using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
		response.EnsureSuccessStatusCode();

		using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
		await foreach (var update in ReadStreamingResponse(stream, cancellationToken))
		{
			yield return update;
		}
	}

	/// <summary>
	/// Simple string-based streaming chat completion for convenience.
	/// </summary>
	/// <param name="prompt">The user prompt</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Streaming text updates</returns>
	public async IAsyncEnumerable<string> GetStreamingResponseAsync(string prompt, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(prompt))
			throw new ArgumentException("Prompt cannot be null or empty.", nameof(prompt));

		var messages = new[] { new ChatMessage(ChatRole.User, prompt) };
		await foreach (var update in GetStreamingResponseAsync(messages, null, cancellationToken))
		{
			yield return update.Text ?? string.Empty;
		}
	}
	/// <summary>
	/// List available models from the Apertus API.
	/// Equivalent to:
	/// curl --request GET \
	///      --url https://api.publicai.co/v1/models \
	///      --header 'Authorization: <string>' \
	///      --header 'User-Agent: <string>'
	/// </summary>
	public async Task<IEnumerable<Model>> ListModelsAsync(CancellationToken cancellationToken = default)
	{
		var request = new HttpRequestMessage(HttpMethod.Get, _modelsEndpoint);

		using var response = await _httpClient.SendAsync(request, cancellationToken);

		if (!response.IsSuccessStatusCode)
		{
			var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
			throw new HttpRequestException($"Failed to fetch models: {(int)response.StatusCode} {response.ReasonPhrase}\n{errorContent}");
		}

		var json = await response.Content.ReadAsStringAsync(cancellationToken);
		using var doc = JsonDocument.Parse(json);
		var root = doc.RootElement;

		if (!root.TryGetProperty("data", out var dataArray) || dataArray.ValueKind != JsonValueKind.Array)
		{
			throw new InvalidOperationException("Unexpected response format: 'data' array missing.");
		}

		return dataArray
			.EnumerateArray()
			.Select(m => new Model
			{
				Id = m.GetProperty("id").GetString() ?? string.Empty,
				Object = m.GetProperty("object").GetString() ?? string.Empty,
				Created = DateTimeOffset.FromUnixTimeSeconds(m.GetProperty("created").GetInt64()).DateTime,
				OwnedBy = m.GetProperty("owned_by").GetString() ?? string.Empty
			})
			.ToList();
	}
	public object? GetService(Type serviceType, object? serviceKey = null) => null;
	public void Dispose()
	{
		_httpClient?.Dispose();
	}
	private HttpRequestMessage CreateChatRequest(object payload)
	{
		return new HttpRequestMessage(HttpMethod.Post, _chatEndpoint)
		{
			Content = new StringContent(
				JsonSerializer.Serialize(payload),
				Encoding.UTF8,
				"application/json")
		};
	}

	private static async IAsyncEnumerable<ChatResponseUpdate> ReadStreamingResponse(Stream stream, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		using var reader = new StreamReader(stream);
		while (!reader.EndOfStream)
		{
			var line = await reader.ReadLineAsync();
			if (line is null || !line.StartsWith("data:")) continue;

			var json = line.Substring(6).Trim();
			if (json == "[DONE]") yield break;

			using var doc = JsonDocument.Parse(json);
			var root = doc.RootElement;

			if (root.GetProperty("choices")[0].TryGetProperty("delta", out var delta) &&
				delta.TryGetProperty("content", out var contentElement))
			{
				var content = contentElement.GetString();
				if (!string.IsNullOrEmpty(content))
				{
					yield return new ChatResponseUpdate(ChatRole.Assistant, content);
				}
			}
		}
	}
}