using System;
namespace libCBUI
{
	public interface ITemplated
	{
		ConsoleColor? Background { get; set; }
		ConsoleColor? Foreground { get; set; }

		BorderStyle Border { get; set; }

		Thickness Padding { get; set; }
	}
}
