using Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Repositories
{
	public interface ICinemasRepository : IDisposable
	{
		Task<IEnumerable<Cinema>> GetCinemasByIdsAndDateAsync(ICollection<short> ids, ICollection<DateTime> dates);
		Task SaveCinemasAsync(IEnumerable<Cinema> cinemas);
		Task SaveFilmsAsync(Cinema cinema, IEnumerable<Film> films);
		Task SaveShowsAsync(Cinema cinema, Film film, IEnumerable<DateTime> shows);
		Task DeleteCinemasAsync(IEnumerable<short> ids);
		Task DeleteShowsAsync(short cinemaId);
	}
}
