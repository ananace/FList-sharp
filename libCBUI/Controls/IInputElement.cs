using System;

namespace libCBUI.Controls
{
	public interface IInputElement
	{
		event EventHandler OnFocusGained;
		event EventHandler OnFocusLost;

		event EventHandler<Events.ConsoleKeyEventArgs> OnKeyInput;

		bool Focusable { get; }
		bool IsEnabled { get; }
		bool IsEffectivelyEnabled { get; }
		bool IsFocused { get; }

		void Focus();
		void RaiseEvent(EventArgs ev);
	}
}
