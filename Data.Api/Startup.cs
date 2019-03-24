using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace Data.Api
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

			services
				.Configure<Options.DockerSecrets>(Configuration.GetSection(nameof(Options.DockerSecrets)))
				.Configure<Options.Database>(Configuration.GetSection(nameof(Options.Database)));

			services
				.AddTransient<Repositories.ICinemasRepository, Repositories.Concrete.CinemasRepository>();

			services
				.AddTransient<Services.ICinemasService, Services.Concrete.CinemasService>();

			// Swagger services.
			services.AddSwaggerGen(options =>
				options.SwaggerDoc("v1", new Info { Title = "Data.Api", Version = "v1", })
			);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app
				.UseSwagger()
				.UseSwaggerUI(options =>
					options.SwaggerEndpoint("/swagger/v1/swagger.json", "Data.Api")
				);

			app.UseMvc();
		}
	}
}
