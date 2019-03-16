using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Data.Api.Controllers
{
	[Route("")]
	[ApiController]
	public class AliveController : ControllerBase
	{
		private readonly IHostingEnvironment _hostingEnvironment;

		public AliveController(IHostingEnvironment hostingEnvironment)
		{
			_hostingEnvironment = hostingEnvironment;
		}

		[HttpGet]
		public IActionResult Get()
		{
			return Ok(
				new
				{
					_hostingEnvironment.ApplicationName,
					_hostingEnvironment.EnvironmentName,
					ControllerName = this.GetType().Name,
					Environment.MachineName,
					ServerTime = DateTime.Now,
				});
		}
	}
}
