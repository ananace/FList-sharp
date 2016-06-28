using System;

namespace libCBUI.Controls
{
	public static class ControlExtensions
	{
		public static IControl GetTopmostParent(this IControl control)
		{
			var top = control;
			var cur = control;

			while (cur != null)
			{
				top = cur;
				cur = cur.Parent;
			}

			return top;
		}
	}
}
