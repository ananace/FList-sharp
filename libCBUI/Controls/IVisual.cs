using System;

namespace libCBUI.Controls
{
	public interface IVisual
	{
		Rect Bounds { get; }

		ConsoleColor EffectiveBackground { get; }
		ConsoleColor EffectiveForeground { get; }

		bool IsEffectivelyVisible { get; }
		bool IsVisible { get; }

		void InvalidateVisual();
		void Render();
	}
}
