using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Data.Api.Tests.Controllers
{
	public class AliveControllerTests
	{
		[Fact]
		public async Task AliveControllerTests_Get_ReturnsJsonOfValues()
		{
			// Arrange
			string response;

			using (var testApi = new TestApi())
			{
				// Act
				(_, _, response) = await testApi.SendAsync(HttpMethod.Get, "/");
			}

			Assert.False(string.IsNullOrWhiteSpace(response));

			dynamic json = JsonConvert.DeserializeObject(response);

			Assert.NotNull(json.applicationName);
			Assert.NotNull(json.environmentName);
			Assert.NotNull(json.controllerName);
			Assert.NotNull(json.machineName);
			Assert.NotNull(json.serverTime);
		}
	}
}
