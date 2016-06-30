using System;

namespace libCBUI.Controls
{
	public interface IVisual
	{
		Rect Bounds { get; }
		Rect EffectiveBounds { get; }

		bool IsVisible { get; }

		void InvalidateVisual();
		void Render();
	}
}
