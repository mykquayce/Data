using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Data.Api
{
	public static class Program
	{
		public static Task Main(string[] args)
		{
			return WebHost.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration((context, configurationBuilder) =>
				{
					configurationBuilder
						.AddDockerSecrets(optional: context.HostingEnvironment.IsDevelopment(), reloadOnChange: true);
				})
				.UseStartup<Startup>()
				.Build()
				.RunAsync();
		}
	}
}
