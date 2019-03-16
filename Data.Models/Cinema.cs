using System.Collections.Generic;

namespace Data.Models
{
	public class Cinema
	{
		public short Id { get; set; }
		public string Name { get; set; }
		public IList<Film> Films { get; } = new List<Film>();

		public override string ToString() => $"{Name} ({Id:D}): {Films.Count:D} show(s)";
	}
}
