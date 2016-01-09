using System;
using ConsoleMessenger.Types;

namespace ConsoleMessenger.UI.Panels
{
	public class HorizontalPanel : Panel
	{
		internal override Point GetChildOffset(Control control)
		{
			if (!Children.Contains(control))
				return base.GetChildOffset(control);

			var cid = Children.IndexOf(control);
			var offset = base.GetChildOffset(control);

			for (int i = cid - 1; i >= 0; --i)
			{
				offset += new Size(Children[i].Size.Width, 0);
			}

			return offset;
		}
	}
}

