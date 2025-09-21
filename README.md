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
