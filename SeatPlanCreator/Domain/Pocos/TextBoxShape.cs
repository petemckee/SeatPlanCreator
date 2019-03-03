using Spire.Presentation;

namespace SeatPlanCreatorWeb.Domain.Pocos
{
	public sealed class TextBoxShape
	{
		public IAutoShape AutoShape { get; }
		public int Index { get; }

		public TextBoxShape(IAutoShape autoShape, int index)
		{
			AutoShape = autoShape;
			Index = index;
		}
	}
}