using System.Collections.Generic;
using System.Linq;

namespace SeatPlanCreatorWeb.Domain.Pocos
{
	public sealed class WorkSheetData
	{
		public IEnumerable<Student> Students { get; set; }
		public string GroupName { get; set; }
		public IEnumerable<string> Warnings { get; set; }
		public IEnumerable<string> Errors { get; set; }

		public WorkSheetData()
		{
			Warnings = Enumerable.Empty<string>();
			Errors = Enumerable.Empty<string>();
		}
	}
}