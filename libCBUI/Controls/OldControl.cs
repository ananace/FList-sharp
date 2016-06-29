using System;

namespace libCBUI.Controls
{
	public abstract class OldControl
	{
		Point _Position;
		Size _Size;
		SizingHint _SizeHint;

		bool _IsActive = true;
		bool _HasFocus;
		bool _NeedsRedraw = true;
		bool _Visible = true;
		bool _UpdatingLayout;

		public BorderStyle Border { get; set; }

		internal string BorderBrush
		{
			get
			{
				switch (Border)
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
		internal char ShadowBrush
		{
			get
			{
				switch (Shadow)
				{
					case ShadowStyle.Light:
						return '░';
					case ShadowStyle.Medium:
						return '▒';
					case ShadowStyle.Dark:
						return '▓';

					default:
						return '\0';
				}
			}
		}

		public ConsoleColor? Background { get; set; }
		public ConsoleColor Foreground { get; set; } = ConsoleColor.White;
		public ConsoleColor? InactiveBackground { get; set; } = ConsoleColor.DarkGray;
		public ConsoleColor InactiveForeground { get; set; } = ConsoleColor.Gray;
		public ConsoleColor? FocusedBackground { get; set; } = ConsoleColor.Gray;
		public ConsoleColor FocusedForeground { get; set; } = ConsoleColor.White;

		public ShadowStyle Shadow { get; set; }

		public object Tag { get; set; }
		public IControl Parent { get; internal set; }

		public SizingHint Sizing
		{ 
			get { return _SizeHint; }
			set
			{
				if (_SizeHint == value) return;

				_SizeHint = value;
				InvalidateLayout();
			}
		}
		public Point Position
		{
			get { return _Position; }
			set
			{
				if (_Position == value) return;

				_Position = value;
				InvalidateLayout();
			}
		}
		public Size Size
		{
			get { return _Size; }
			set
			{
				if (_Size == value) return;

				_Size = value;
				InvalidateLayout();
			}
		}

		public Rect Rect
		{
			get
			{
				return new Rect(_Position, _Size);
			}
		}

		public virtual Rect Margin { get; set; }
		public virtual Rect Padding { get; set; }

		public bool IsActive
		{
			get { return _IsActive; }
			set
			{
				_IsActive = value;

				if (!value)
					IsFocused &= !_HasFocus;

				InvalidateVisual();
			}
		}

		public bool IsFocused
		{
			get { return _HasFocus; }
			set
			{
				if (IsFocusable && value != _HasFocus)
				{
					if (_HasFocus)
					{
						//TopmostParent.Children.ForEach(c => c.IsFocused = false);
						OnFocusGained?.Invoke(this, EventArgs.Empty);
					}
					else
						OnFocusLost?.Invoke(this, EventArgs.Empty);

					_HasFocus = value;
				}
			}
		}

		public bool IsVisible
		{
			get
			{
				return _Visible;
			}
			set
			{
				if (value != _Visible)
				{
					_Visible = value;
					InvalidateVisual();
				}
			}
		}

		internal virtual bool IsFocusable { get { return false; } }
		internal virtual bool ShouldDraw { get { return _Visible && (Parent == null || Parent.ShouldDraw); } }
		internal virtual bool NeedsRedraw { get { return _NeedsRedraw; } }

		public virtual Point DisplayPosition
		{
			get
			{
				return Parent == null ? Position : Parent.DisplayPosition + Parent.GetChildOffset(this) + Position + Margin.Position;
			}
		}

		public event EventHandler OnVisualInvalidated;
		public event EventHandler OnFocusGained;
		public event EventHandler OnFocusLost;

		public void Dispose()
		{
			Parent = null;
		}

		public virtual void Focus()
		{
			IsFocused = true;
		}

		public virtual void PushInput(ConsoleKeyInfo key)
		{

		}

		public virtual void InvalidateLayout()
		{
			if (_UpdatingLayout) return;

			_UpdatingLayout = true;
			try
			{
				if (_SizeHint == SizingHint.FillAvailable)
				{
					if (Parent != null)
						Size = Parent.Size - Parent.Padding.Size;
					else
						Size = Graphics.AvailableSize;
				}
				else if (_SizeHint == SizingHint.ShrinkToFit)
				{
					SizeToContent();
				}

				InvalidateVisual();
			}
			finally
			{
				_UpdatingLayout = false;
			}
		}

		public virtual void SizeToContent()
		{

		}

		public virtual void InvalidateVisual()
		{
			if (!_NeedsRedraw && OnVisualInvalidated != null)
				OnVisualInvalidated(this, EventArgs.Empty);
			_NeedsRedraw = true;

			if (Parent != null)
				Parent.InvalidateVisual();
		}

		internal virtual Point GetChildOffset(BaseControl control)
		{
			if (Border == BorderStyle.None)
				return new Point(0, 0);
			return new Point(1, 1);
		}

		protected void Clear()
		{
			Graphics.DrawFilledBox(DisplayPosition, Size, ' ', Background 
			                       ?? Parent?.Background 
			                       ?? TopmostParent.Background 
			                       ?? ConsoleColor.Black);
		}

		public void Draw(bool force = false)
		{
			if (!force && (!ShouldDraw || !_NeedsRedraw))
				return;

			_NeedsRedraw = false;

			lock (Console.Out)
			{
				using (new CursorChanger(new Point(DisplayPosition.X, DisplayPosition.Y)))
				using (new ColorChanger())
				{
					if (Border != BorderStyle.None)
					{
						if (!Background.HasValue)
							Graphics.DrawBox(DisplayPosition, Size, BorderBrush, Background ?? Parent?.Background
											 ?? TopmostParent.Background ?? ConsoleColor.Black, Foreground);

						Console.SetCursorPosition(DisplayPosition.X + 1, DisplayPosition.Y + 1);
					}

					var background = (!_IsActive ? InactiveBackground : (_HasFocus ? FocusedBackground : Background))
						?? Background;
					
					if (background.HasValue)
					{
						Console.BackgroundColor = background.Value;
						Point borderOffset = (Border == BorderStyle.None ? new Point(0, 0) : new Point(1, 1));
						Size borderSize = (Border == BorderStyle.None ? new Size(0, 0) : new Size(2, 2));

						if (Size >= borderSize)
							Graphics.DrawFilledBox(DisplayPosition + borderOffset, Size - borderSize, ' ', background.Value);
					}
					else if (Parent != null && Parent.Background.HasValue)
						Console.BackgroundColor = Parent.Background.Value;
					else if (TopmostParent.Background.HasValue)
						Console.BackgroundColor = TopmostParent.Background.Value;

					Console.ForegroundColor = Foreground;

					if (Shadow != ShadowStyle.None)
					{
						Graphics.DrawLine(DisplayPosition + new Size(0, Size.Height + 1),
							DisplayPosition + Size + new Size(1, 1),
							Parent != null ? Parent.Background ?? ConsoleColor.Black : ConsoleColor.Black,
							Console.BackgroundColor,
							ShadowBrush);
						Graphics.DrawLine(DisplayPosition + new Size(Size.Width + 1, 0),
							DisplayPosition + Size + new Size(1, 1),
							Parent != null ? Parent.Background ?? ConsoleColor.Black : ConsoleColor.Black,
							Console.BackgroundColor,
							ShadowBrush);
					}

					Render();
				}
			}
		}

		public abstract void Render();
	}
}

