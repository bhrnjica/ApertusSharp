using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

using ApertusSharp.SemanticKernel;

class Program
{
	static async Task Main(string[] args)
	{
		await UserBasicApertusFeatures(args);

		//await UseServiceCollectionWithApertusFeatures(args);

		//await UserApertusInSemantickernel(args);
	}


	private static async Task UserBasicApertusFeatures(string[] args)
	{
		var apiKey = Environment.GetEnvironmentVariable("APERTUS_TOKEN");
		if (string.IsNullOrEmpty(apiKey))
		{
			Console.WriteLine("❌ Please set the APERTUS_TOKEN environment variable.");
			return;
		}

		var apertus = new ApertusClient(
			apiKey: apiKey,
			model: "swiss-ai/apertus-8b-instruct"
		);


		var messages = new List<ChatMessage>
		{
			new ChatMessage(ChatRole.User, "Hello!")
		};

		if (args.Contains("stream"))
		{
			Console.Write("AI: ");

			await foreach (var chunk in apertus.GetStreamingResponseAsync(messages))
			{
				Console.Write(chunk);
			}
			Console.WriteLine("\n Streaming complete.");
		}
		else
		{
			var response = await apertus.GetResponseAsync(messages);
			Console.WriteLine("AI: " + response);
		}

		//List models
		if (args.Contains("listmodels"))
		{
			var models = await apertus.ListModelsAsync();
			Console.WriteLine("Available models:");
			foreach (var model in models)
			{
				Console.WriteLine($"- {model.Id} (owned by {model.OwnedBy}, created at {model.Created})");
			}
		}
		//Use simple chat interface
		if (args.Contains("simple"))
		{
			var msg = "Hello, how are you?";
			Console.WriteLine("User: " + msg);
			var response = await apertus.GetResponseAsync(msg);
			Console.WriteLine("AI: " + response);
		}


	}
	private static async Task UseServiceCollectionWithApertusFeatures(string[] args)
	{
		var apiKey = Environment.GetEnvironmentVariable("APERTUS_TOKEN");
		if (string.IsNullOrEmpty(apiKey))
		{
			Console.WriteLine("❌ Please set the APERTUS_TOKEN environment variable.");
			return;
		}

		var services = new ServiceCollection();
		services.AddApertusChatClient(apiKey: apiKey, model: "swiss-ai/apertus-8b-instruct");

		await using var provider = services.BuildServiceProvider(new ServiceProviderOptions
		{
			ValidateScopes = true,
			ValidateOnBuild = true
		});
		var apertus = provider.GetRequiredService<ApertusClient>();

		var messages = new List<ChatMessage>
					{
						new ChatMessage(ChatRole.User, "Hello from ServiceCollection!")
					};

		if (args.Contains("stream"))
		{
			Console.Write("AI: ");
			await foreach (var chunk in apertus.GetStreamingResponseAsync(messages))
			{
				Console.Write(chunk);
			}
			Console.WriteLine("\n Streaming complete.");
		}
		else
		{
			var response = await apertus.GetResponseAsync(messages);
			Console.WriteLine("AI: " + response);
		}
	}
	private static async Task UserApertusInSemantickernel(string[] args)
	{
		var apiKey = Environment.GetEnvironmentVariable("APERTUS_TOKEN");
		if (string.IsNullOrEmpty(apiKey))
		{
			Console.WriteLine("❌ Please set the APERTUS_TOKEN environment variable.");
			return;
		}

		// Create ApertusClient
		var apertus = new ApertusClient(apiKey: apiKey,	model: "swiss-ai/apertus-8b-instruct");

		// Create a Semantic Kernel builder
		var builder = Kernel.CreateBuilder();

		// Register Apertus as a chat client service
		builder.Services.AddApertusChatClient(apiKey: apiKey, model: "swiss-ai/apertus-8b-instruct");

		var kernel = builder.Build();

		// Use the kernel to get a chat completion
		var chat = kernel.GetRequiredService<IChatClient>();

		var msg = "How does Semantic Kernel work with Apertus?";
		Console.WriteLine("User: " + msg);
		
		var history = new List<ChatMessage>
			{
				new ChatMessage(ChatRole.User, msg)
			};

		var result = await chat.GetResponseAsync(history);

		Console.WriteLine("AI: " + result.Text);
	}
}
