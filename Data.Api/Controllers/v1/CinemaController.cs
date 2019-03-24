using Dawn;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Data.Api.Controllers.v1
{
	[Route("v1/[controller]")]
	[ApiController]
	public class CinemaController : ControllerBase
	{
		private readonly Repositories.ICinemasRepository _cinemasRepository;
		private readonly Services.ICinemasService _cinemasService;

		public CinemaController(
			Repositories.ICinemasRepository cinemasRepository,
			Services.ICinemasService cinemasService)
		{
			_cinemasRepository = Guard.Argument(() => cinemasRepository).NotNull().Value;
			_cinemasService = Guard.Argument(() => cinemasService).NotNull().Value;
		}

		[HttpGet]
		[Route("")]
		[ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<Models.Cinema>))]
		[ProducesResponseType((int)HttpStatusCode.NotFound)]
		[ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(Exception))]
		public async Task<IActionResult> GetCinemasByIdsAndDateAsync(
			[FromQuery] IEnumerable<short> id,
			[FromQuery] IEnumerable<Models.DaysOfWeek> day,
			[FromQuery] IEnumerable<Models.TimesOfDay> timeOfDay,
			[FromQuery] IEnumerable<string> title)
		{
			try
			{
				var results = (await _cinemasService.GetCinemasAsync(id, day, timeOfDay, title)).ToList();

				if (results.Count == 0)
				{
					return NotFound();
				}

				return Ok(results);
			}
			catch (Exception exception)
			{
				return StatusCode(
					(int)HttpStatusCode.InternalServerError,
					exception);
			}
		}

		[HttpPost]
		[Route("")]
		[ProducesResponseType((int)HttpStatusCode.OK)]
		[ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(Exception))]
		public async Task<IActionResult> SaveCinemasAsync([FromBody] ICollection<Models.Cinema> cinemas)
		{
			try
			{
				await _cinemasRepository.SaveCinemasAsync(cinemas);
				return Ok();
			}
			catch (Exception exception)
			{
				return StatusCode(
					(int)HttpStatusCode.InternalServerError,
					exception);
			}
		}

		[HttpDelete]
		[ProducesResponseType((int)HttpStatusCode.OK)]
		[ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(Exception))]
		public async Task<IActionResult> DeleteCinemasAsync([FromQuery]ICollection<short> id)
		{
			try
			{
				await _cinemasRepository.DeleteCinemasAsync(id);
				return Ok();
			}
			catch (Exception exception)
			{
				return StatusCode(
					(int)HttpStatusCode.InternalServerError,
					exception);
			}
		}
	}
}
