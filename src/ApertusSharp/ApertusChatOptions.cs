// Ignore Spelling: Apertus

namespace ApertusSharp
{
	/// <summary>
	/// Configuration options for the Apertus chat client.
	/// </summary>
	public class ApertusChatOptions
	{
		/// <summary>
		/// The Apertus API endpoint URL.
		/// </summary>
		public string Endpoint { get; set; } = "https://api.publicai.co";

		/// <summary>
		/// The API key for authenticating with the Apertus service.
		/// </summary>
		public string? ApiKey { get; set; }

		/// <summary>
		/// The default model to use for chat completions.
		/// </summary>
		public string Model { get; set; } = "swiss-ai/apertus-8b-instruct";

		/// <summary>
		/// Custom HttpClient to use for requests. If null, a default one will be created.
		/// </summary>
		public HttpClient? HttpClient { get; set; }
	}
}