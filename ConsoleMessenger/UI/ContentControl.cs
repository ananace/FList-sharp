using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleMessenger.UI
{
	public abstract class ContentControl : Control
	{
		object _Content;

		public object Content
		{
			get { return _Content; }
			set
			{
				if (_Content == value) return;

				_Content = value;

				if (OnContentChanged != null)
					OnContentChanged(this, EventArgs.Empty);
			}
		}

		public event EventHandler OnContentChanged;
		
	}
}
