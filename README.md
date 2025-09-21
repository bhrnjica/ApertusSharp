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

Coming soon via NuGet. For now, clone the repo:

```bash
git clone https://github.com/your-username/ApertusSharp.git
```

## 🚀 Quick Start

Register the Apertus client:

```csharp
services.AddApertusChatClient(options =>
{
    options.Endpoint = "https://api.swis-ai.ch/apertus";
    options.ApiKey = configuration["SwisAI:ApiKey"];
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
        var response = await _chatClient.GetChatCompletionAsync(prompt);
        return response.Content;
    }
}
```

## 🔌 Semantic Kernel Integration

ApertusSharp can be used as a custom `IChatCompletionService` for Semantic Kernel:

```csharp
builder.Services.AddSingleton<IChatCompletionService, ApertusChatCompletionService>();
```

## 🧱 Architecture

- `IChatClient` and `IChatCompletionService` abstractions
- Streaming via `IAsyncEnumerable<ChatMessage>`
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
```

---

Let me know if you'd like me to generate a matching `Directory.Build.props`, `Program.cs`, or DI extension class next. I can also help scaffold the Semantic Kernel adapter.
