using System;

namespace libCBUI.Events
{
	public class ConsoleKeyEventArgs : EventArgs
	{
		ConsoleKeyInfo _Key;

		public ConsoleKeyInfo Key => _Key;

		public ConsoleKeyEventArgs(ConsoleKeyInfo Key)
		{
			_Key = Key;
		}
	}
}
