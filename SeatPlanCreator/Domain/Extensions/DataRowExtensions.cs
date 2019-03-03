using System.Data;

namespace SeatPlanCreatorWeb.Domain.Extensions
{
	static class DataRowExtensions
	{
		public static string GetString(this DataRow row, int colIndex)
		{

			if (colIndex == -1)
				return string.Empty;

			var obj = row.ItemArray[colIndex];
			return obj?.ToString();
		}
	}
}