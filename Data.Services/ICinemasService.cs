using Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Services
{
	public interface ICinemasService
	{
		Task<IEnumerable<Cinema>> GetCinemasAsync(
			IEnumerable<short> ids,
			IEnumerable<DaysOfWeek> days,
			IEnumerable<TimesOfDay> timesOfDays,
			IEnumerable<string> titles);
	}
}
