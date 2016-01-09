using System;
using ConsoleMessenger.Types;

namespace ConsoleMessenger.UI.Panels
{
	public class VerticalPanel : Panel
	{
		int _RefreshTemp;

		public override Point InternalPosition { get {
				return base.InternalPosition + new Point(0, _RefreshTemp);
			}
		}
		public override Point InternalSize { get {
				return base.InternalSize - new Point(0, _RefreshTemp);
			}
		}

		public new void Refresh()
		{
			base.Refresh();

			_RefreshTemp = 0;
			foreach (var child in Children)
			{
				child.Refresh();
				_RefreshTemp += child.DrawSize.Y;
			}
		}
	}
}

