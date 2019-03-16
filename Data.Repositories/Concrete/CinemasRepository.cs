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

			var userId = dockerSecrets.MySqlCineworldUser ?? database.MySqlCineworldUser;
			var password = dockerSecrets.MySqlCineworldPassword ?? database.MySqlCineworldPassword;

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

		public async Task<IEnumerable<Cinema>> GetCinemasByIdsAndDateAsync(ICollection<short> ids, ICollection<DateTime> dates)
		{
			var sql = @"select c.id cinemaId, c.name cinemaName, s.time showTime, f.edi filmEdi, f.title filmTitle
				from `show` s
					join `film` f on s.filmEdi = f.edi
					join `cinema` c on s.cinemaId = c.id
				where 1=1";

			if (ids.Count > 0)
			{
				sql += " and c.id in @ids";
			}

			if (dates.Count > 0)
			{
				sql += " and date(s.time) in @dates";
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

				film.Shows.Add(cinemaDto.ShowTime);
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

				tasks.Add(SaveFilmsAsync(cinema, cinema.Films));
			}

			return Task.WhenAll(tasks);
		}

		public Task SaveFilmsAsync(Cinema cinema, IEnumerable<Film> films)
		{
			var tasks = new List<Task>();

			foreach (var film in films)
			{
				tasks.Add(_connection.ExecuteAsync(
					"cineworld.film_insert",
					new { _edi = film.Edi, _title = film.Title, },
					commandType: CommandType.StoredProcedure));

				tasks.Add(SaveShowsAsync(cinema, film, film.Shows));
			}

			return Task.WhenAll(tasks);
		}

		public Task SaveShowsAsync(Cinema cinema, Film film, IEnumerable<DateTime> shows)
		{
			var tasks = new List<Task>();

			foreach (var show in shows)
			{
				tasks.Add(_connection.ExecuteAsync(
					"cineworld.show_insert",
					new { _cinemaId = cinema.Id, _filmEdi = film.Edi, _time = show, },
					commandType: CommandType.StoredProcedure));
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
