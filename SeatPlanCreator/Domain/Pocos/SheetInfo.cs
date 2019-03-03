using System.Collections.Generic;

namespace SeatPlanCreatorWeb.Domain.Pocos
{

	public class SheetInfo
	{
		public string GroupName { get; set; }
		public IEnumerable<string> ColumnNames { get; set; }
	}
}