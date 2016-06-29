using System;

namespace libCBUI.Controls
{
	public interface IStyled
	{
		ConsoleColor? Background { get; set; }
		ConsoleColor? Foreground { get; set; }

		BorderStyle Border { get; set; }

		Thickness Padding { get; set; }
	}
}
