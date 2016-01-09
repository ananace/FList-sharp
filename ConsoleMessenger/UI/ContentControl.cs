using ConsoleMessenger.Types;
using System;

namespace ConsoleMessenger.UI
{
	public class ContentControl : Control
	{
		public enum SizingHint
		{
			SizeToContent,
			FixedSize
		}

		object _Content;
		public SizingHint Sizing { get; set; }

		public virtual object Content
		{
			get { return _Content; }
			set
			{
				if (_Content == value) return;

				_Content = value;

				if (OnContentChanged != null)
					OnContentChanged(this, EventArgs.Empty);

				InvalidateVisual();

				if (Sizing == SizingHint.SizeToContent)
					SizeToContent();
			}
		}

		protected string ContentDrawable
		{
			get
			{
				return (Content ?? "").ToString();
			}
		}

		public event EventHandler OnContentChanged;

		public void SizeToContent()
		{
			var toDraw = Content.ToString();
			Size = new Size(toDraw.Length, 1);
		}

		public override void Render()
		{
			var toDraw = ContentDrawable;
			if (toDraw.Length > Size.Width)
				toDraw = toDraw.Substring(0, Size.Width);

			Console.Write(toDraw);
		}
	}
}
