namespace libCBUI.Controls
{
	public interface ILayoutable : IVisual
	{
		Size DesiredSize { get; }

		int Width { get; }
		int Height { get; }
		int MinWidth { get; }
		int MinHeight { get; }
		int MaxWidth { get; }
		int MaxHeight { get; }

		Thickness Margin { get; }

		HorizontalAlignment HorizontalAlignment { get; }
		VerticalAlignment VerticalAlignment { get; }
	}
}
