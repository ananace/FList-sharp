using System;

namespace libCBUI.Controls
{
	public interface IInputElement
	{
		event EventHandler OnFocusGained;
		event EventHandler OnFocusLost;

		event EventHandler<ConsoleKeyInfo> OnKeyInput;

		bool Focusable { get; }
		bool IsEnabled { get; }
		bool IsEffectivelyEnabled { get; }
		bool IsFocused { get; }

		void Focus();
	}
}
