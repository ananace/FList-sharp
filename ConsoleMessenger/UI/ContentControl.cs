﻿using ConsoleMessenger.Types;
using System;

namespace ConsoleMessenger.UI
{
	public class ContentControl : Control
	{
		object _Content;

		public virtual object Content
		{
			get { return _Content; }
			set
			{
				if (_Content == value) return;

				_Content = value;

				if (OnContentChanged != null)
					OnContentChanged(this, EventArgs.Empty);

				InvalidateLayout();
			}
		}

		protected string ContentDrawable
		{
			get
			{
				var drawable = (Content ?? "").ToString();

				var length = drawable.Length;
				var allowed = Size.Width;
				if (length > allowed)
				{
					return allowed <= 0 ? "" : drawable.Substring(length - allowed, allowed);
				}
				return drawable;
			}
		}

		public virtual event EventHandler OnContentChanged;

		public override void SizeToContent()
		{
			var toDraw = Content.ToString();

			int maxSize = Parent != null
				? (Parent.Size - Parent.Padding.Size - (Size)DisplayPosition).Width 
				: (Graphics.AvailableSize).Width;

            Size = Size.Constrain(new Size(toDraw.ANSILength(), 1), new Size(maxSize, 1));
		}

		public override void Render()
		{
			var toDraw = ContentDrawable;
			Graphics.WriteANSIString(toDraw);
		}
	}
}
