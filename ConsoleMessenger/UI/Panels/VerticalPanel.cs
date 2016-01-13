using System.Collections.Generic;
using ConsoleMessenger.Types;

namespace ConsoleMessenger.UI.Panels
{
	public class VerticalPanel : Panel
	{
		public new IList<Control> Children { get { return base.Children; } }

		internal override Point GetChildOffset(Control control)
		{
			if (!base.Children.Contains(control))
				return base.GetChildOffset(control);

			var cid = base.Children.IndexOf(control);
			var offset = base.GetChildOffset(control);

			for (int i = cid - 1; i >= 0; --i)
			{
				var child = base.Children[i];
				offset += new Size(0, child.Size.Height + child.Margin.Height);
			}

			return offset;
		}
	}
}

