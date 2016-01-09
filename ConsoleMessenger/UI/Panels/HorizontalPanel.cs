using System;
using ConsoleMessenger.Types;

namespace ConsoleMessenger.UI.Panels
{
	public class HorizontalPanel : Panel
	{
		int _RefreshTemp;

		public override Point InternalPosition { get {
				return base.InternalPosition + new Point(_RefreshTemp, 0);
			}
		}
		public override Point InternalSize { get {
				return base.InternalSize - new Point(_RefreshTemp, 0);
			}
		}

		public new void Refresh()
		{
			base.Refresh();

			_RefreshTemp = 0;
			foreach (var child in Children)
			{
				child.Refresh();
				_RefreshTemp += child.DrawSize.X;
			}
		}
	}
}

