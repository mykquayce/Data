using Dapper;
using Data.Models;
using Dawn;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Repositories.Concrete
{
	public class CinemasRepository : ICinemasRepository
	{
		private readonly IDbConnection _connection;

		public CinemasRepository(
			IOptions<Options.DockerSecrets> dockerSecretsOptions,
			IOptions<Options.Database> databaseOptions)
		{
			var dockerSecrets = Guard.Argument(() => dockerSecretsOptions).NotNull().Value.Value;
			var database = Guard.Argument(() => databaseOptions).NotNull().Value.Value;

			var userId = dockerSecrets.MySqlCineworldUser ?? database.MySqlCineworldUser ?? throw new ArgumentNullException(nameof(Options.DockerSecrets.MySqlCineworldUser));
			var password = dockerSecrets.MySqlCineworldPassword ?? database.MySqlCineworldPassword ?? throw new ArgumentNullException(nameof(Options.DockerSecrets.MySqlCineworldPassword));

			var connectionString = $"server={database.Server};port={database.Port:D};user id={userId};password={password};database={database.Name};";

			_connection = new MySqlConnection(connectionString);
		}

		public void Dispose()
		{
			_connection?.Dispose();
		}

		public Task DeleteCinemasAsync(IEnumerable<short> ids)
		{
			var tasks = new List<Task>();

			foreach (var id in ids)
			{
				tasks.Add(_connection.ExecuteAsync(
					"cineworld.cinema_delete",
					new { _id = id, },
					commandType: CommandType.StoredProcedure));

				tasks.Add(DeleteShowsAsync(id));
			}

			return Task.WhenAll(tasks);
		}

		public Task DeleteShowsAsync(short cinemaId)
		{
			return _connection.ExecuteAsync(
				"cineworld.show_delete",
				new { _cinemaId = cinemaId, },
				commandType: CommandType.StoredProcedure);
		}

		public async Task<IEnumerable<Cinema>> GetCinemasAsync(
			IReadOnlyCollection<short> ids = default,
			IReadOnlyCollection<DateTime> dates = default,
			IReadOnlyCollection<(TimeSpan, TimeSpan)> times = default,
			IReadOnlyCollection<string> titles = default)
		{
			var sql = @"select c.id cinemaId, c.name cinemaName, s.time showTime, f.edi filmEdi, f.title filmTitle
				from `cineworld`.`show` s
					join `cineworld`.`film` f on s.filmEdi = f.edi
					join `cineworld`.`cinema` c on s.cinemaId = c.id
				where 1=1";

			if (ids?.Count > 0)
			{
				sql += " and c.id in @ids";
			}

			if (dates?.Count > 0)
			{
				sql += " and date(s.time) in @dates";
			}

			if (times?.Count > 0)
			{
				sql += " and (";

				sql += string.Join(" or ", from t in times
										   select $@"(time(s.time) >= '{t.Item1:hh\:mm\:ss}' and time(s.time) <= '{t.Item2:hh\:mm\:ss}')");

				sql += ")";
			}

			if (titles?.Count > 0)
			{
				sql += " and (";
				sql += string.Join(" or ", titles.Select(t => $"(f.title like '%{t}%')"));
				sql += ")";
			}

			var cinemaDtos = await _connection.QueryAsync<CinemaDto>(sql, new { ids, dates, });

			var cinemas = new List<Cinema>();

			foreach (var cinemaDto in cinemaDtos)
			{
				var cinema = cinemas.SingleOrDefault(c => c.Id == cinemaDto.CinemaId);

				if (cinema == default)
				{
					cinema = new Cinema { Id = cinemaDto.CinemaId, Name = cinemaDto.CinemaName, };
					cinemas.Add(cinema);
				}

				var film = cinema.Films.SingleOrDefault(f => f.Edi == cinemaDto.FilmEdi);

				if (film == default)
				{
					film = new Film { Edi = cinemaDto.FilmEdi, Title = cinemaDto.FilmTitle, };
					cinema.Films.Add(film);
				}

				film.Shows.Add(DateTime.SpecifyKind(cinemaDto.ShowTime, DateTimeKind.Utc));
			}

			return cinemas;
		}

		public Task SaveCinemasAsync(IEnumerable<Cinema> cinemas)
		{
			var tasks = new List<Task>();

			foreach (var cinema in cinemas)
			{
				tasks.Add(_connection.ExecuteAsync(
					"cineworld.cinema_insert",
					new { _id = cinema.Id, _name = cinema.Name, },
					commandType: CommandType.StoredProcedure));

				foreach (var film in cinema.Films)
				{
					if (film.Edi == default) // Theatre Let
					{
						continue;
					}

					tasks.Add(_connection.ExecuteAsync(
						"cineworld.film_insert",
						new { _edi = film.Edi, _title = film.Title, },
						commandType: CommandType.StoredProcedure));

					foreach (var show in film.Shows)
					{
						tasks.Add(_connection.ExecuteAsync(
							"cineworld.show_insert",
							new { _cinemaId = cinema.Id, _filmEdi = film.Edi, _time = show, },
							commandType: CommandType.StoredProcedure));
					}
				}
			}

			return Task.WhenAll(tasks);
		}

		private class CinemaDto
		{
			public short CinemaId { get; set; }
			public string CinemaName { get; set; }
			public DateTime ShowTime { get; set; }
			public int FilmEdi { get; set; }
			public string FilmTitle { get; set; }
		}
	}
}
