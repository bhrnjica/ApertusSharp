using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

using Microsoft.Extensions.AI;


var apiKey = Environment.GetEnvironmentVariable("APERTUS_TOKEN");
if (string.IsNullOrEmpty(apiKey))
{
	Console.WriteLine("❌ Please set the APERTUS_TOKEN environment variable.");
	return;
}

var apertus = new ApertusClient(
	apiKey: "zpka_804c882501e2488aa5cf135953919738_78c61f8d",
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

