using System;

namespace libCBUI.Controls
{
	public abstract class InputElement : IInputElement
	{
		public abstract bool Focusable { get; }

		public bool IsEnabled { get; set; }
		public abstract bool IsEffectivelyEnabled { get; }

		public bool IsFocused { get; private set; }

		public event EventHandler OnFocusGained;
		public event EventHandler OnFocusLost;
		public event EventHandler<Events.ConsoleKeyEventArgs> OnKeyInput;

		public void Focus()
		{
			if (!Focusable || !IsEffectivelyEnabled)
				return;

			FocusManager.GetInstance(this).CurrentFocus = this;
		}

		protected virtual void PreFocusGained() { }
		protected virtual void PreFocusLost() { }
		protected virtual void PreKeyInput(ConsoleKeyInfo key) { }

		public void RaiseEvent(EventArgs ev)
		{
			if (ev is Events.FocusGainedEventArgs)
			{
				PreFocusGained();
				IsFocused = true;
				OnFocusGained?.Invoke(this, ev);
			}
			else if (ev is Events.FocusLostEventArgs)
			{
				PreFocusLost();
				IsFocused = false;
				OnFocusLost?.Invoke(this, ev);
			}
			else if (ev is Events.ConsoleKeyEventArgs)
			{
				PreKeyInput((ev as Events.ConsoleKeyEventArgs).Key);
				OnKeyInput?.Invoke(this, ev as Events.ConsoleKeyEventArgs);
			}
		}
	}
}
