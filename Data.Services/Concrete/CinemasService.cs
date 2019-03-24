using Data.Models;
using Dawn;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Services.Concrete
{
	public class CinemasService : ICinemasService
	{
		private readonly Repositories.ICinemasRepository _repository;

		public CinemasService(
			Repositories.ICinemasRepository repository)
		{
			_repository = Guard.Argument(() => repository).NotNull().Value;
		}

		public Task<IEnumerable<Cinema>> GetCinemasAsync(
			IEnumerable<short> ids,
			IEnumerable<DaysOfWeek> days,
			IEnumerable<TimesOfDay> timesOfDays,
			IEnumerable<string> titles)
		{
			var dates = days.Aggregate().ToDayOfWeeks().ToDates(range: 100).ToList();

			var times = timesOfDays.Aggregate().ToTimeSpans().ToList();

			return _repository.GetCinemasAsync(
				ids.ToList(),
				dates,
				times,
				titles.ToList());
		}
	}
}
