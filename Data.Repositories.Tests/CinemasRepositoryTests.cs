using Data.Repositories.Concrete;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Data.Repositories.Tests
{
	public class CinemasRepositoryTests : IDisposable
	{
		private readonly ICinemasRepository _repo;

		public CinemasRepositoryTests()
		{
			var database = new Options.Database
			{
				Server = "localhost",
				Port = 3306,
				Name = "cineworld",
				MySqlCineworldUser = "poB2V9jjAf94TubQ8P5nc6H2Ei8ZqLZayUWTrchpJLAoWwSWWEzGJgJTlE6zetjSpXIlihpcoDkIlg",
				MySqlCineworldPassword = "9xX9wz61aConBYmdR4szF8OE5BfLo5A9kqn7L6R0D9In5Sb83TPjDNjURyQ0zTrYdXOjy06wDv4tiQWk6D1A3V3Oz6pamLlflHvfbpAauvv4NkXGLvMkGWCW5hrXihA538NWtWNsFcwjXEqtFLPJulJYiix3nkYgtxcH0bbuzgXbVRFxJPBX7ZRYJ8HlYmGugq981q2mWKi02PYtTYsAJ5zHYdJ6ibdXNSAB6PguyOuxA0lSRZz3MYO9Tk5vuUhI3L",
			};

			var dockerSecrets = new Options.DockerSecrets();

			_repo = new CinemasRepository(
				Mock.Of<IOptions<Options.DockerSecrets>>(o => o.Value == dockerSecrets),
				Mock.Of<IOptions<Options.Database>>(o => o.Value == database));
		}

		public void Dispose() => _repo?.Dispose();

		[Theory]
		[InlineData(new short[] { 23, }, new int[0])]
		[InlineData(new short[] { 23, }, new[] { 0, })]
		[InlineData(new short[] { 23, 96, 57, }, new[] { 0, 1, })]
		public async Task CinemasRepositoryTests_GetCinemaAsync_ReturnsPopulatedCinemas(IReadOnlyCollection<short> ids, ICollection<int> days)
		{
			// Arrange
			var today = DateTime.UtcNow.Date;
			var dateTimes = days.Select(d => today.AddDays(d)).ToList();

			// Act
			var actual = await _repo.GetCinemasAsync(ids, dateTimes, default, default);

			// Assert
			Assert.NotNull(actual);
			Assert.NotEmpty(actual);
			Assert.All(actual, Assert.NotNull);
			Assert.All(actual.Select(c => c.Name), Assert.NotNull);
			Assert.All(actual.Select(c => c.Name), Assert.NotEmpty);
			Assert.All(actual.Select(c => c.Films), Assert.NotNull);
			Assert.All(actual.Select(c => c.Films), Assert.NotEmpty);
			Assert.All(actual.SelectMany(c => c.Films), Assert.NotNull);
			Assert.All(actual.SelectMany(c => c.Films), f => Assert.NotNull(f.Title));
			Assert.All(actual.SelectMany(c => c.Films), f => Assert.NotEmpty(f.Title));
			Assert.All(actual.SelectMany(c => c.Films), f => Assert.NotEmpty(f.Shows));
			Assert.All(actual.SelectMany(c => c.Films), f => Assert.All(f.Shows, s => Assert.NotEqual(default, s)));
		}

		/*[Theory]
		[InlineData("htdpvnhtvsntvsrneiutvsrneuintseuirvznhsvtrcnhvtxsnhevxstznevtsrzxnhhnlueituilrtasfunileaswfrtunile")]
		public async Task CinemasRepositoryTests_SaveFilmsAsync_WithTooLongATitle_DoesntSwallowTheError(string title)
		{
			var films = new[] { new Models.Film { Edi = int.MaxValue, Title = title, } };

			Exception exception = default;

			try
			{
				await _repo.SaveFilmsAsync(default, films);
				Assert.True(1 == 0);
			}
			catch (Exception ex)
			{
				exception = ex;
			}

			Assert.NotNull(exception);
			Assert.IsType<MySql.Data.MySqlClient.MySqlException>(exception);

			var mySqlException = (MySql.Data.MySqlClient.MySqlException)exception;

			Assert.StartsWith("Data too long for column '_title' at row ", mySqlException.Message);
		}*/
	}
}
