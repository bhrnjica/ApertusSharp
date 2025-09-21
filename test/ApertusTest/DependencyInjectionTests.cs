using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.AI;
using ApertusSharp;

namespace ApertusTest;

public class DependencyInjectionTests
{
    [Fact]
    public void AddApertusChatClient_RegistersServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApertusChatClient(options =>
        {
            options.ApiKey = "test-key";
            options.Model = "test-model";
            options.Endpoint = "https://test.endpoint";
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var chatClient = serviceProvider.GetService<IChatClient>();
        var apertusClient = serviceProvider.GetService<IApertusApiClient>();

        Assert.NotNull(chatClient);
        Assert.NotNull(apertusClient);
        Assert.IsType<ApertusClient>(chatClient);
        Assert.Same(chatClient, apertusClient);
    }

    [Fact]
    public void AddApertusChatClient_WithStringParameters_RegistersServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApertusChatClient("test-key", "test-model", "https://test.endpoint");

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var chatClient = serviceProvider.GetService<IChatClient>();
        var apertusClient = serviceProvider.GetService<IApertusApiClient>();

        Assert.NotNull(chatClient);
        Assert.NotNull(apertusClient);
        Assert.IsType<ApertusClient>(chatClient);
    }

    [Fact]
    public void AddApertusChatClient_WithoutApiKey_ThrowsException()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddApertusChatClient(options =>
        {
            // Intentionally not setting ApiKey
            options.Model = "test-model";
        });

        var serviceProvider = services.BuildServiceProvider();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
        {
            serviceProvider.GetRequiredService<IChatClient>();
        });
    }
}