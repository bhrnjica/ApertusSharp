
using ApertusSharp;
namespace ApertusTest;


public class UnitTest1
{
	[Fact]
	public async Task Test_Returs_List_Of_Models()
	{
		var apiKey = Environment.GetEnvironmentVariable("APERTUS_TOKEN");
		Assert.False(string.IsNullOrEmpty(apiKey), "Set the APERTUS_TOKEN environment variable to run the test.");

		var aClient = new ApertusClient(apiKey);

		var models = await aClient.ListModelsAsync();
		
		Assert.NotNull(models);
		Assert.NotEmpty(models);
		Assert.Contains(models, m => m.Id == "swiss-ai/apertus-8b-instruct");

	}

	[Fact]
	public async Task TestSimple_Chat()
	{
		var apiKey = Environment.GetEnvironmentVariable("APERTUS_TOKEN");
		Assert.False(string.IsNullOrEmpty(apiKey), "Set the APERTUS_TOKEN environment variable to run the test.");

		var apertus = new ApertusClient(model:"swiss-ai/apertus-8b-instruct", apiKey:apiKey);

		await foreach (var stream in apertus.GenerateAsync("How are you today?"))
			Assert.False(string.IsNullOrEmpty(stream.Text));

	}
}