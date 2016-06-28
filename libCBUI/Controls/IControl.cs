namespace libCBUI
{
	public interface IControl : IInputElement, ILayoutable, INamed, IVisual
	{
		IControl Parent { get; }
	}
}
