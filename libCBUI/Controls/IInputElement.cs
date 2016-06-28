using System;
namespace libCBUI
{
	public interface IInputElement
	{
		event EventHandler OnFocusGained;
		event EventHandler OnFocusLost;

		event EventHandler OnKeyInput;

		bool Focusable { get; }
		bool IsEnabled { get; }
		bool IsEffectivelyEnabled { get; }
		bool IsFocused { get; }

		void Focus();
	}
}
