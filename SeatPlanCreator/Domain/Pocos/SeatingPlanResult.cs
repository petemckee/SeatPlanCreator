using Spire.Presentation;

namespace SeatPlanCreatorWeb.Domain.Pocos
{
	public sealed class SeatingPlanResult
	{
		public string FileName { get; }
		public Presentation Presentation { get; set; }

		public bool Success { get; }

		public string FileData { get; set; }

		public SeatingPlanResult(string fileName, Presentation presentation, bool success)
		{
			FileName = fileName;
			Presentation = presentation;
			Success = success;
		}
	}
}