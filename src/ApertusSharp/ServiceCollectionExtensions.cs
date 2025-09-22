// Ignore Spelling: Apertus

using ApertusSharp;
using ApertusSharp.Interfaces;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
	/// <summary>
	/// Extension methods for configuring Apertus chat services in DI container.
	/// </summary>
	public static class ApertusServiceCollectionExtensions
	{
		/// <summary>
		/// Adds the Apertus chat client to the service collection.
		/// </summary>
		/// <param name="services">The service collection</param>
		/// <param name="configureOptions">Action to configure the Apertus options</param>
		/// <returns>The service collection for chaining</returns>
		public static IServiceCollection AddApertusChatClient(
			this IServiceCollection services,
			Action<ApertusChatOptions> configureOptions)
		{
			if (services == null) throw new ArgumentNullException(nameof(services));
			if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

			services.Configure(configureOptions);

			services.TryAddSingleton<IChatClient>(serviceProvider =>
			{
				var options = serviceProvider.GetRequiredService<IOptions<ApertusChatOptions>>().Value;

				if (string.IsNullOrEmpty(options.ApiKey))
					throw new InvalidOperationException("ApiKey must be configured in ApertusChatOptions");

				return new ApertusClient(
					model: options.Model,
					apiKey: options.ApiKey,
					endpoint: options.Endpoint,
					httpClient: options.HttpClient);
			});

			services.TryAddSingleton<IApertusApiClient>(serviceProvider =>
				serviceProvider.GetRequiredService<IChatClient>() as IApertusApiClient
				?? throw new InvalidOperationException("IChatClient is not an IApertusApiClient"));

			services.TryAddSingleton<ApertusClient>(sp => (ApertusClient)sp.GetRequiredService<IChatClient>());

			return services;
		}

		/// <summary>
		/// Adds the Apertus chat client to the service collection with minimal configuration.
		/// </summary>
		/// <param name="services">The service collection</param>
		/// <param name="apiKey">The API key for the Apertus service</param>
		/// <param name="model">The model to use (optional)</param>
		/// <param name="endpoint">The endpoint URL (optional)</param>
		/// <returns>The service collection for chaining</returns>
		public static IServiceCollection AddApertusChatClient(
			this IServiceCollection services,
			string apiKey,
			string model = "swiss-ai/apertus-8b-instruct",
			string endpoint = "https://api.publicai.co")
		{
			return services.AddApertusChatClient(options =>
			{
				options.ApiKey = apiKey;
				options.Model = model;
				options.Endpoint = endpoint;
			});
		}
	}
}