using ApertusSharp.Entities;
using Microsoft.Extensions.AI;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ApertusSharp
{
	public interface IApertusApiClient
	{
		Task<IEnumerable<Model>> ListModelsAsync(CancellationToken cancellationToken = default);
		IAsyncEnumerable<ChatResponseUpdate> GenerateAsync(string prompt, CancellationToken cancellationToken = default);

	}
}