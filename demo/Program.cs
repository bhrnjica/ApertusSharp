
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using System.IO;

class Program
{
	static async Task Main(string[] args)
	{
		while (true)
		{
			Console.WriteLine("ApertusSharp - running Swiss-Ai on .NET. Choose an example to run:");
			Console.WriteLine(Environment.NewLine);
			Console.WriteLine("1. CreateApertusChatClient");
			Console.WriteLine("2. UseBasicApertusFeatures");
			Console.WriteLine("3. UseApertusFromServiceCollection");
			Console.WriteLine("4. UseApertusInSemantickernel");
			Console.WriteLine("5. UseApertusFromSpectreConsole");
			Console.WriteLine("0. Exit");
			Console.Write("Enter your choice: ");
			var input = Console.ReadLine();

			switch (input)
			{
				case "1":
					await CreateApertusChatClient();
					break;
				case "2":
					await UseBasicApertusFeatures(args);
					break;
				case "3":
					await UseApertusFromServiceCollection(args);
					break;
				case "4":
					await UseApertusInSemantickernel(args);
					break;
				case "5":
					await UseApertusFromSpectreConsole(args);
					break;
				case "0":
					return;
				default:
					Console.WriteLine("Invalid choice. Please try again.");
					break;
			}

			Console.WriteLine();
		}
	}


	private static async Task UseBasicApertusFeatures(string[] args)
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

		while (true)
		{
			Console.WriteLine("Choose an action:");
			Console.WriteLine("1. List available models");
			Console.WriteLine("2. Run chat (streaming)");
			Console.WriteLine("3. Run chat (non-streaming)");
			Console.WriteLine("0. Back to main menu");
			Console.Write("Enter your choice: ");
			var input = Console.ReadLine();

			switch (input)
			{
				case "1":
					var models = await apertus.ListModelsAsync();
					Console.WriteLine("Available models:");
					foreach (var model in models)
					{
						Console.WriteLine($"- {model.Id} (owned by {model.OwnedBy}, created at {model.Created})");
					}
					break;
				case "2":
					Console.Write("Enter your message: ");
					var streamMsg = Console.ReadLine() ?? string.Empty;
					var streamMessages = new List<ChatMessage>
								{
									new ChatMessage(ChatRole.User, streamMsg)
								};
					Console.Write("AI: ");
					await foreach (var chunk in apertus.GetStreamingResponseAsync(streamMessages))
					{
						Console.Write(chunk);
					}
					Console.WriteLine("\nStreaming complete.");
					break;
				case "3":
					Console.Write("Enter your message: ");
					var msg = Console.ReadLine() ?? string.Empty;
					var messages = new List<ChatMessage>
								{
									new ChatMessage(ChatRole.User, msg)
								};
					var response = await apertus.GetResponseAsync(messages);
					Console.WriteLine("AI: " + response);
					break;
				case "0":
					return;
				default:
					Console.WriteLine("Invalid choice. Please try again.");
					break;
			}

			Console.WriteLine();
		}
	}
	private static async Task CreateApertusChatClient()
	{
		var apiKey = Environment.GetEnvironmentVariable("APERTUS_TOKEN");
		if (string.IsNullOrEmpty(apiKey))
		{
			Console.WriteLine("❌ Please set the APERTUS_TOKEN environment variable.");
			return;
		}
		var apertus = new ApertusClient(model: "swiss-ai/apertus-8b-instruct", apiKey: apiKey);

		var msg = "How are you today?";
		Console.WriteLine("User " + msg);
		Console.Write("AI: ");
		await foreach (var stream in apertus.GenerateAsync(msg))
			Console.Write(stream.Text);
	}
	private static async Task UseApertusFromServiceCollection(string[] args)
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
		var msg = "Hello from ServiceCollection!";
		Console.WriteLine("User: " + msg);

		var messages = new List<ChatMessage>
			{
				new ChatMessage(ChatRole.User, msg)
			};

		Console.Write("AI: ");
		await foreach (var chunk in apertus.GetStreamingResponseAsync(messages))
		{
			Console.Write(chunk);
		}
		Console.WriteLine("\n Streaming complete.");
	}
	private static async Task UseApertusInSemantickernel(string[] args)
	{
		var apiKey = Environment.GetEnvironmentVariable("APERTUS_TOKEN");
		if (string.IsNullOrEmpty(apiKey))
		{
			Console.WriteLine("❌ Please set the APERTUS_TOKEN environment variable.");
			return;
		}

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
	private static async Task UseApertusFromSpectreConsole(string[] args)
	{

		var apiKey = Environment.GetEnvironmentVariable("APERTUS_TOKEN");
		if (string.IsNullOrEmpty(apiKey))
		{
			Spectre.Console.AnsiConsole.MarkupLine("[red]❌ Please set the APERTUS_TOKEN environment variable.[/]");
			return;
		}

		var apertus = new ApertusClient(
			apiKey: apiKey,
			model: "swiss-ai/apertus-8b-instruct"
		);

		var chatHistory = new List<ChatMessage>();
		Spectre.Console.AnsiConsole.MarkupLine("[green]Apertus Chat Console. Type 'exit' to quit.[/]\n");

		while (true)
		{
			var userInput = Spectre.Console.AnsiConsole.Ask<string>("[yellow]You:[/]");
			if (string.Equals(userInput, "exit", StringComparison.OrdinalIgnoreCase))
				break;

			chatHistory.Add(new ChatMessage(ChatRole.User, userInput));

			Spectre.Console.AnsiConsole.Markup("[blue]AI:[/] ");

			await foreach (var chunk in apertus.GetStreamingResponseAsync(chatHistory))
			{
				if(chunk != null && chunk.Text != null)
					Spectre.Console.AnsiConsole.Write(chunk.Text);
			}

			Spectre.Console.AnsiConsole.WriteLine();

			// Add last AI response to history for context
			var lastResponse = await apertus.GetResponseAsync(chatHistory);
			chatHistory.Add(new ChatMessage(ChatRole.Assistant, lastResponse.Text));
		}
	}
}
