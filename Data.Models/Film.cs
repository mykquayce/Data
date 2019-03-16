using System;
using System.Collections.Generic;

namespace Data.Models
{
	public class Film
	{
		public int Edi { get; set; }
		public string Title { get; set; }
		public IList<DateTime> Shows { get; } = new List<DateTime>();

		public override string ToString() => $"{Title} ({Edi:D}): {Shows.Count:D} show(s)";
	}
}
