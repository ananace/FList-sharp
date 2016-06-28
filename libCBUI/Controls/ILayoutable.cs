using System;
namespace libCBUI
{
	public interface ILayoutable
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
