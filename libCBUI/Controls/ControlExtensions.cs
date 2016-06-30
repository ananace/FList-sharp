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


		public static ConsoleColor GetEffectiveBackground(this IControl control)
		{
			ConsoleColor? background = control.GetSelfAndParents()
				.Where(p => (p is IStyled))
				.Select(p => (p as IStyled).Background)
				.FirstOrDefault(b => b.HasValue);

			return background.HasValue ? background.Value : ConsoleColor.Black;
		}

		public static ConsoleColor GetEffectiveForeground(this IControl control)
		{
			ConsoleColor? foreground = control.GetSelfAndParents()
				.Where(p => (p is IStyled))
				.Select(p => (p as IStyled).Foreground)
				.FirstOrDefault(f => f.HasValue);

			return foreground.HasValue ? foreground.Value : ConsoleColor.White;
		}

		public static bool IsEffectivelyEnabled(this IControl control)
		{
			return control.GetSelfAndParents().All(p => p.IsEnabled);
		}

		public static bool IsEffectivelyVisible(this IControl control)
		{
			return control.GetSelfAndParents().All(p => p.IsVisible);
		}
	}
}
