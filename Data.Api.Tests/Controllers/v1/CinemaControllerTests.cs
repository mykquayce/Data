using Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Data.Api.Tests.Controllers.v1
{
	public class CinemaControllerTests
	{
		[Theory]
		[InlineData(new[] { 23, })]
		[InlineData(new[] { 23, 57, })]
		[InlineData(new[] { 23, 57, 96, })]
		public async Task CinemaControllerTests_GetCinemasByIdsAndDateAsync_ReturnsCinemas(int[] cinemaIds)
		{
			var today = DateTime.UtcNow.Date;
			var sixMonths = today.AddMonths(6);
			HttpStatusCode statusCode;
			string json;

			var url = "/v1/cinema?";

			url += string.Join("&", from cinemaId in cinemaIds
									select "id=" + cinemaId);

			using (var testApi = new TestApi())
			{
				(statusCode, json) = await testApi.SendAsync(HttpMethod.Get, url);
			}

			Assert.Equal(HttpStatusCode.OK, statusCode);

			Assert.NotNull(json);
			Assert.NotEmpty(json);
			Assert.Equal('[', json[0]);

			var cinemas = JsonConvert.DeserializeObject<ICollection<Cinema>>(json);

			Assert.NotEmpty(cinemas);
			Assert.Equal(cinemaIds.Length, cinemas.Count);
			Assert.All(cinemas, Assert.NotNull);
			Assert.All(cinemas.Select(c => c.Id), id => Assert.InRange(id, 1, short.MaxValue));
			Assert.All(cinemas.Select(c => c.Id), id => Assert.Contains(id, cinemaIds));
			Assert.All(cinemas.Select(c => c.Name), Assert.NotNull);
			Assert.All(cinemas.Select(c => c.Name), Assert.NotEmpty);
			Assert.All(cinemas.Select(c => c.Films), Assert.NotNull);
			Assert.All(cinemas.Select(c => c.Films), Assert.NotEmpty);
			Assert.All(cinemas.SelectMany(c => c.Films), Assert.NotNull);
			Assert.All(cinemas.SelectMany(c => c.Films).Select(f => f.Edi), edi => Assert.InRange(edi, 1, int.MaxValue));
			Assert.All(cinemas.SelectMany(c => c.Films).Select(f => f.Title), Assert.NotNull);
			Assert.All(cinemas.SelectMany(c => c.Films).Select(f => f.Title), Assert.NotEmpty);
			Assert.All(cinemas.SelectMany(c => c.Films).Select(f => f.Shows), Assert.NotNull);
			Assert.All(cinemas.SelectMany(c => c.Films).Select(f => f.Shows), Assert.NotEmpty);
			Assert.All(cinemas.SelectMany(c => c.Films).SelectMany(f => f.Shows), dt => Assert.InRange(dt, today, sixMonths));
			Assert.All(cinemas.SelectMany(c => c.Films).SelectMany(f => f.Shows), dt => Assert.Equal(0, dt.Second));
			Assert.All(cinemas.SelectMany(c => c.Films).SelectMany(f => f.Shows), dt => Assert.Equal(0, dt.Millisecond));
		}

		[Fact]
		public async Task CinemaControllerTests_SaveCinemasAsync_SavesCinemas()
		{
			var today = DateTime.UtcNow.Date;
			HttpStatusCode statusCode;
			string responseJson;

			var cinemas = new[]
			{
				new Cinema
				{
					Id = 30_000,
					Name = "pdtfin.tprdnzifndrtpfezsh.irnih.dfepztuls",
					Films =
					{
						new Film
						{
							Edi = 1_000_000,
							Title = "prdgfnunhidprsgtkdnyspgtheikrodnrksyp",
							Shows =
							{
								today.AddMinutes(64),
								today.AddMinutes(145),
								today.AddMinutes(547),
								today.AddMinutes(556),
								today.AddMinutes(743),
							},
						},
					},
				},
			};

			var requestJson = JsonConvert.SerializeObject(cinemas);

			using (var testApi = new TestApi())
			{
				(statusCode, responseJson) = await testApi.SendAsync(HttpMethod.Post, "/v1/cinema", requestJson);
				Assert.Equal(HttpStatusCode.OK, statusCode);
				Assert.Empty(responseJson);

				(statusCode, responseJson) = await testApi.SendAsync(HttpMethod.Get, "/v1/cinema?id=30000");
				Assert.Equal(HttpStatusCode.OK, statusCode);
				Assert.NotEmpty(responseJson);

				var cinemasResponse = JsonConvert.DeserializeObject<ICollection<Cinema>>(responseJson);

				Assert.NotEmpty(cinemas);
				Assert.All(cinemas, Assert.NotNull);
				Assert.All(cinemas.Select(c => c.Id), id => Assert.InRange(id, 1, short.MaxValue));
				Assert.All(cinemas.Select(c => c.Name), Assert.NotNull);
				Assert.All(cinemas.Select(c => c.Name), Assert.NotEmpty);
				Assert.All(cinemas.Select(c => c.Films), Assert.NotNull);
				Assert.All(cinemas.Select(c => c.Films), Assert.NotEmpty);
				Assert.All(cinemas.SelectMany(c => c.Films), Assert.NotNull);
				Assert.All(cinemas.SelectMany(c => c.Films).Select(f => f.Edi), edi => Assert.InRange(edi, 1, int.MaxValue));
				Assert.All(cinemas.SelectMany(c => c.Films).Select(f => f.Title), Assert.NotNull);
				Assert.All(cinemas.SelectMany(c => c.Films).Select(f => f.Title), Assert.NotEmpty);
				Assert.All(cinemas.SelectMany(c => c.Films).Select(f => f.Shows), Assert.NotNull);
				Assert.All(cinemas.SelectMany(c => c.Films).Select(f => f.Shows), Assert.NotEmpty);
				Assert.All(cinemas.SelectMany(c => c.Films).SelectMany(f => f.Shows), dt => Assert.InRange(dt, today, DateTime.MaxValue));
				Assert.All(cinemas.SelectMany(c => c.Films).SelectMany(f => f.Shows), dt => Assert.Equal(0, dt.Second));
				Assert.All(cinemas.SelectMany(c => c.Films).SelectMany(f => f.Shows), dt => Assert.Equal(0, dt.Millisecond));

				(statusCode, responseJson) = await testApi.SendAsync(HttpMethod.Delete, "/v1/cinema?id=30000");
				Assert.Equal(HttpStatusCode.OK, statusCode);
				Assert.Empty(responseJson);
			}
		}
	}
}
