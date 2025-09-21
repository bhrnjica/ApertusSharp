using ApertusSharp.Entities;
using Microsoft.Extensions.AI;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ApertusSharp
{
	/// <summary>
	/// Interface for Apertus-specific API operations beyond the standard IChatClient.
	/// </summary>
	public interface IApertusApiClient
	{
		/// <summary>
		/// Lists all available models from the Apertus API.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>Collection of available models</returns>
		Task<IEnumerable<Model>> ListModelsAsync(CancellationToken cancellationToken = default);
		
		/// <summary>
		/// Generates streaming chat response updates for a simple text prompt.
		/// </summary>
		/// <param name="prompt">The user prompt</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>Streaming chat response updates</returns>
		IAsyncEnumerable<ChatResponseUpdate> GenerateAsync(string prompt, CancellationToken cancellationToken = default);

	}
}