using System;

namespace libCBUI.Controls
{
	public interface IFocusManager
	{
		IInputElement CurrentFocus { get; set; }
	}
}
