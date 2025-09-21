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

Register the Apertus client:

```csharp
services.AddApertusChatClient(options =>
{
    options.Endpoint = "https://api.publicai.co"; // Apertus API endpoint
    options.ApiKey = configuration["SwisAI:ApiKey"];
    options.Model = "swiss-ai/apertus-8b-instruct";
});
```

Use it in your service:

```csharp
public class ChatService
{
    private readonly IChatClient _chatClient;

    public ChatService(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<string> AskAsync(string prompt)
    {
        var response = await _chatClient.GetResponseAsync(new[] { new ChatMessage(ChatRole.User, prompt) });
        return response.Messages.FirstOrDefault()?.Text ?? string.Empty;
    }
}
```

## 📓 Jupyter Notebook Integration

ApertusSharp can be used seamlessly in Jupyter Notebooks with the .NET Interactive kernel. This enables interactive data science and AI exploration workflows.

### Setup

1. **Install .NET Interactive kernel for Jupyter:**
   ```bash
   dotnet tool install -g Microsoft.dotnet-interactive
   dotnet interactive jupyter install
   ```

2. **Start Jupyter and create a new C# notebook:**
   ```bash
   jupyter notebook
   # or use Jupyter Lab for a more modern interface
   jupyter lab
   ```
   Select "C#" as the kernel when creating a new notebook.

### Usage in Jupyter Notebooks

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

**Cell 5 - Streaming response:**
```csharp
Console.Write("AI: ");
await foreach (var chunk in apertus.GetStreamingResponseAsync("Tell me a short story about AI"))
{
    Console.Write(chunk);
}
Console.WriteLine();
```

**Cell 6 - List available models:**
```csharp
var models = await apertus.ListModelsAsync();
foreach (var model in models)
{
    Console.WriteLine($"- {model.Id} (owned by {model.OwnedBy})");
}
```

### Tips for Jupyter Notebooks

- Set your API key as an environment variable for security: `Environment.GetEnvironmentVariable("APERTUS_TOKEN")`
- Use `display()` function to render rich outputs
- Break complex workflows into multiple cells for better interactivity
- Leverage async/await for responsive notebook experience

## 🔌 Semantic Kernel Integration

ApertusSharp can be used as a custom `IChatClient` for Semantic Kernel:

```csharp
builder.Services.AddSingleton<IChatClient, ApertusChatCompletionService>();

// Or register directly via DI extensions
builder.Services.AddApertusChatClient(apiKey: "your-api-key");
```

## 🧱 Architecture

- `IChatClient` and `IChatCompletionService` abstractions
- Streaming via `IAsyncEnumerable<ChatResponseUpdate>`
- Extensible options via `ApertusChatOptions`
- Designed for clean DI and modular composition

## 📚 Documentation

- [Swis-AI Apertus API](https://swis-ai.ch/docs)
- [.NET AI SDK](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.ai)
- [Semantic Kernel](https://aka.ms/semantic-kernel)

## 🤝 Contributing

Pull requests welcome! Please open an issue first for major changes.

## 📄 License

MIT — see [LICENSE](LICENSE) for details.
