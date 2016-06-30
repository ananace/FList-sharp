using System;
using System.Collections.Generic;
using System.Linq;

namespace libCBUI.Controls
{
	public static class ControlExtensions
	{
		public static IControl GetTopmostParent(this IControl control)
		{
			return (control.Parents ?? new IControl[0]).LastOrDefault() ?? control;
		}

		public static IEnumerable<IControl> GetSelfAndParents(this IControl control)
		{
			yield return control;

			if (control.Parents != null)
				foreach (var p in control.Parents)
					yield return p;
		}

		public static IEnumerable<IControl> GetSelfAndChildren(this IControl control)
		{
			yield return control;

			if (control.Children != null)
				foreach (var p in control.Children)
					yield return p;
		}

		public static string GetBorderBrush(this IStyled styled)
		{
			switch (styled.Border)
			{
			case BorderStyle.Filled:
				return "████ ████";
			case BorderStyle.BlockOutline:
				return "▄▄▄█ █▀▀▀";
			case BorderStyle.Outline:
				return "┌─┐│ │└─┘";
			case BorderStyle.DoubleOutline:
				return "╔═╗║ ║╚═╝";

			default:
				return null;
			}
		}
	}
}
