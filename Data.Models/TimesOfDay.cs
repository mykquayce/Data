using System;

namespace Data.Models
{
	[Flags]
	public enum TimesOfDay : byte
	{
		None = 0,

		Night = 1,
		Morning = 2,

		AM = Night | Morning,

		Afternoon = 4,
		Evening = 8,

		PM = Afternoon | Evening,

		AllDay = AM | PM,
	}
}
