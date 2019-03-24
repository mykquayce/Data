using System;
using System.Collections.Generic;
using System.Linq;

namespace Data.Models
{
	public static class ExtensionMethods
	{
		public static IEnumerable<DayOfWeek> ToDayOfWeeks(this DaysOfWeek daysOfWeek)
		{
			if ((daysOfWeek & DaysOfWeek.Sunday) != 0) yield return DayOfWeek.Sunday;
			if ((daysOfWeek & DaysOfWeek.Monday) != 0) yield return DayOfWeek.Monday;
			if ((daysOfWeek & DaysOfWeek.Tuesday) != 0) yield return DayOfWeek.Tuesday;
			if ((daysOfWeek & DaysOfWeek.Wednesday) != 0) yield return DayOfWeek.Wednesday;
			if ((daysOfWeek & DaysOfWeek.Thursday) != 0) yield return DayOfWeek.Thursday;
			if ((daysOfWeek & DaysOfWeek.Friday) != 0) yield return DayOfWeek.Friday;
			if ((daysOfWeek & DaysOfWeek.Saturday) != 0) yield return DayOfWeek.Saturday;
		}

		public static DaysOfWeek ToDaysOfWeek(this DayOfWeek dayOfWeek)
		{
			switch (dayOfWeek)
			{
				case DayOfWeek.Sunday: return DaysOfWeek.Sunday;
				case DayOfWeek.Monday: return DaysOfWeek.Monday;
				case DayOfWeek.Tuesday: return DaysOfWeek.Tuesday;
				case DayOfWeek.Wednesday: return DaysOfWeek.Wednesday;
				case DayOfWeek.Thursday: return DaysOfWeek.Thursday;
				case DayOfWeek.Friday: return DaysOfWeek.Friday;
				case DayOfWeek.Saturday: return DaysOfWeek.Saturday;
				default:
					throw new ArgumentOutOfRangeException(nameof(dayOfWeek), dayOfWeek, $"Unexpected {nameof(dayOfWeek)}: {dayOfWeek}")
					{
						Data = { [nameof(dayOfWeek)] = dayOfWeek },
					};
			}
		}

		public static DaysOfWeek Aggregate(this IEnumerable<DaysOfWeek> dayOfWeeks)
			=> dayOfWeeks.Aggregate(DaysOfWeek.None, (sum, next) => sum |= next);

		public static DaysOfWeek ToDaysOfWeek(this IEnumerable<DayOfWeek> dayOfWeeks)
			=> dayOfWeeks.Aggregate(DaysOfWeek.None, (sum, next) => sum |= next.ToDaysOfWeek());

		public static IEnumerable<DateTime> ToDates(this IEnumerable<DayOfWeek> dayOfWeeks, short range = 100)
		{
			var today = DateTime.UtcNow.Date;

			return from i in Enumerable.Range(0, range)
				   let date = today.AddDays(i)
				   where dayOfWeeks.Contains(date.DayOfWeek)
				   select date;
		}

		public static TimesOfDay Aggregate(this IEnumerable<TimesOfDay> timesOfDays)
			=> timesOfDays.Aggregate(TimesOfDay.None, (sum, next) => sum |= next);

		public static IEnumerable<(TimeSpan, TimeSpan)> ToTimeSpans(this TimesOfDay timesOfDay)
		{
			if ((timesOfDay & TimesOfDay.Night) != 0) yield return (TimeSpan.FromHours(0), TimeSpan.FromHours(6));
			if ((timesOfDay & TimesOfDay.Morning) != 0) yield return (TimeSpan.FromHours(6), TimeSpan.FromHours(12));
			if ((timesOfDay & TimesOfDay.Afternoon) != 0) yield return (TimeSpan.FromHours(12), TimeSpan.FromHours(18));
			if ((timesOfDay & TimesOfDay.Evening) != 0) yield return (TimeSpan.FromHours(18), TimeSpan.FromHours(24));
		}
	}
}
