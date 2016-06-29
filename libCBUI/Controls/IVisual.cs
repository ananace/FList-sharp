namespace libCBUI.Controls
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
