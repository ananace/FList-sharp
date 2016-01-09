using System;
using ConsoleMessenger.Types;

namespace ConsoleMessenger.UI.Panels
{
	public class TitledPanel : Panel
	{
		public enum TitleType
		{
			FullBar,
			GroupBox
		}

		public Control TitleControl { get; set; }

		public override Rect Padding
		{
			get
			{
				return Title == TitleType.FullBar ? new Rect(0, 1, 0, 0) : new Rect(2, 2, 2, 2);
			}
				
			set
			{
			}
		}

		public TitleType Title
		{
			get { return Border == BorderStyle.None ? TitleType.FullBar : TitleType.GroupBox; }
			set
			{
				if (value == TitleType.FullBar)
					Border = BorderStyle.None;
				else if (Border == BorderStyle.None)
					Border = BorderStyle.Outline;
			}
		}
		public ConsoleColor? TitleColor { get; set; }

		internal override Point GetChildOffset(Control control)
		{
			if (control == TitleControl)
			{
				switch (Title)
				{
				case TitleType.FullBar:
					return new Point(0, 0);
				case TitleType.GroupBox:
					return new Point(2, 0);
				}
			}

			return new Point(Padding.Left, Padding.Top);
		}

		public override void Render()
		{
			if (Title == TitleType.FullBar)
				Graphics.DrawLine(DisplayPosition, DisplayPosition + new Size(Size.Width, 0), TitleColor ?? ConsoleColor.White);
			else if (TitleControl != null)
				Graphics.DrawLine(DisplayPosition + new Point(2, 0), DisplayPosition + new Point(2, 0) + new Size(TitleControl.Size.Width, 0), TitleColor ?? ConsoleColor.White);

			base.Render();

			if (TitleControl != null)
				TitleControl.Draw();
		}
	}
}

