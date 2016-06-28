using System;
namespace libCBUI
{
	public interface IVisual
	{
		Rect Bounds { get; }

		bool IsEffectivelyVisible { get; }
		bool IsVisible { get; }

		void InvalidateVisual();
		void Render();
	}
}
