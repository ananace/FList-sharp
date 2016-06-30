using System;

namespace libCBUI.Controls
{
	internal class FocusManager : IFocusManager
	{
		static IFocusManager _Global;
		public static IFocusManager GetInstance(IInputElement scope)
		{
			// TODO: Fix scope for multi scope input
			if (_Global == null)
				_Global = new FocusManager();

			return _Global;
		}

		IInputElement _CurrentFocus;
		public IInputElement CurrentFocus
		{
			get { return _CurrentFocus; }
			set
			{
				if (_CurrentFocus == value)
					return;

				if (_CurrentFocus != null)
					_CurrentFocus.RaiseEvent(new Events.FocusLostEventArgs());

				if (value != null)
					value.RaiseEvent(new Events.FocusGainedEventArgs(_CurrentFocus));
				
				_CurrentFocus = value;
			}
		}
	}
}
