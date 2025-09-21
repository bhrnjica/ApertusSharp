# ApertusSharp

ApertusSharp is a modern .NET client for [Swis-AI](https://swis-ai.ch)'s Apertus LLM — built on `Microsoft.Extensions.AI` with full support for Semantic Kernel and custom chat pipelines. It’s designed for clean integration, simple usage, and flexible composition in .NET applications.

## ✨ Features

- ✅ Built on `Microsoft.Extensions.AI` abstractions
- 🔄 Supports both streaming and non-streaming chat
- 🧠 Semantic Kernel compatible
- 🧩 Extensible for custom chat pipelines and agents
- 🧪 Minimal, testable, simple .NET code
- 🧰 Ready for DI registration and service composition


## 📦 Installation

Install via NuGet:

```bash
dotnet add package ApertusSharp
```

Or via Package Manager Console in Visual Studio:

```
Install-Package ApertusSharp
```


## 🚀 Quick Start

### List Available models:

```csharp

var apertus = new ApertusClient(apiKey);
var models = await apertus.ListModelsAsync();
Console.WriteLine($"Available models: {string.Join(", ", models.Select(m => m.Id))}");
```

### Create the Apertus chat client:

```csharp
var apertus = new ApertusClient(model:"swiss-ai/apertus-8b-instruct", apiKey:apiKey);

await foreach (var stream in apertus.GenerateAsync("How are you today?"))
	Console.Write(stream.Text);
```

### Use it as service extension:

```csharp
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

Console.Write("AI: ");
await foreach (var chunk in apertus.GetStreamingResponseAsync(messages))
{
	Console.Write(chunk);
}
Console.WriteLine("\n Streaming complete.");
```


## 🔌 Semantic Kernel Integration

ApertusSharp can be used as a custom `IChatClient` for Semantic Kernel:

```csharp
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
```



## Usage in Jupyter Notebooks

**Cell 1 - Install ApertusSharp package:**
```csharp
#r "nuget: ApertusSharp"
```

**Cell 2 - Import namespaces:**
```csharp
using ApertusSharp;
using Microsoft.Extensions.AI;
```

**Cell 3 - Initialize the client:**
```csharp
var apiKey = "your-api-key-here"; // Replace with your actual API key
var apertus = new ApertusClient(
    apiKey: apiKey,
    model: "swiss-ai/apertus-8b-instruct"
);
```

**Cell 4 - Simple chat interaction:**
```csharp
var response = await apertus.GetResponseAsync("Explain quantum computing in simple terms");
Console.WriteLine(response);
```

### Tips for Jupyter Notebooks

- Set your API key as an environment variable for security: `Environment.GetEnvironmentVariable("APERTUS_TOKEN")`
- Use `display()` function to render rich outputs
- Break complex workflows into multiple cells for better interactivity
- Leverage async/await for responsive notebook experience

## 🧱 Architecture

- `IChatClient` and `IChatCompletionService` abstractions
- Streaming via `IAsyncEnumerable<ChatResponseUpdate>`
- Extensible options via `ApertusChatOptions`
- Designed for clean DI and modular composition

## 📚 Documentation

- [Swis-AI Apertus API](https://platform.publicai.co/api)
- [.NET AI SDK](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.ai)
- [Semantic Kernel](https://aka.ms/semantic-kernel)

## 🤝 Contributing

Pull requests welcome! Please open an issue first for major changes.

## 📄 License

MIT — see [LICENSE](LICENSE) for details.
