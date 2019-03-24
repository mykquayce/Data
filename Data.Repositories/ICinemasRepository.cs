using Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Repositories
{
	public interface ICinemasRepository : IDisposable
	{
		Task<IEnumerable<Cinema>> GetCinemasAsync(
			IReadOnlyCollection<short> ids = default,
			IReadOnlyCollection<DateTime> dates = default,
			IReadOnlyCollection<(TimeSpan, TimeSpan)> times = default,
			IReadOnlyCollection<string> titles = default);

		Task SaveCinemasAsync(IEnumerable<Cinema> cinemas);
		Task DeleteCinemasAsync(IEnumerable<short> ids);
		Task DeleteShowsAsync(short cinemaId);
	}
}
