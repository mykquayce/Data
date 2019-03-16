using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Data.Api.Tests.Controllers
{
	public class TestApi : IDisposable
	{
		private readonly TestServer _server;
		private readonly HttpClient _client;

		public TestApi()
		{
			var builder = WebHost.CreateDefaultBuilder()
				.UseStartup<Startup>()
				.UseEnvironment(EnvironmentName.Development);

			_server = new TestServer(builder);
			_client = _server.CreateClient();
		}

		public void Dispose()
		{
			_client?.Dispose();
			_server?.Dispose();
		}

		public async Task<(HttpStatusCode, string)> SendAsync(HttpMethod method, string relativeUri, string body = default)
		{
			var request = new HttpRequestMessage(method, relativeUri);

			if (!string.IsNullOrWhiteSpace(body))
			{
				request.Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
			}

			var response = await _client.SendAsync(request);

			var content = await response.Content?.ReadAsStringAsync();

			return (response.StatusCode, content);
		}
	}
}
