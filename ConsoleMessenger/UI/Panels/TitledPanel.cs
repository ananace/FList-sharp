using System;
using ConsoleMessenger.Types;

namespace ConsoleMessenger.UI.Panels
{
	public class TitledPanel : Panel
	{
		public Control TitleControl { get; set; }

		public ConsoleColor TitleColor { get; set; }

		bool _InTitleRefresh;
		public override Point InternalSize { get {
				return _InTitleRefresh ? new Point(base.InternalSize.X, 1) : base.InternalSize - new Point(0, 1);
			}
		}
		public override Point InternalPosition { get {
				return _InTitleRefresh ? base.InternalPosition : base.InternalPosition + new Point(0, 1);
			}
		}

		public override void Draw()
		{
			base.Draw();
			if (base.DrawSize.X <= 0 || base.DrawSize.Y <= 0)
				return;
			
			Graphics.DrawLine(DrawPosition, DrawPosition + new Point(DrawSize.X, 0), TitleColor);
			Console.CursorTop++;

			if (TitleControl != null)
				TitleControl.Draw();
		}

		public new void Refresh()
		{
			base.Refresh();

			_InTitleRefresh = true;
			if (TitleControl != null)
				TitleControl.Refresh();
			_InTitleRefresh = false;
		}
	}
}

