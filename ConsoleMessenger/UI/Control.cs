using System;
using ConsoleMessenger.Types;

namespace ConsoleMessenger.UI
{
	public abstract class Control : IDisposable
	{
		public enum BorderStyle
		{
			None,
			Filled,
			BlockOutline,
			Outline,
			DoubleOutline
		}

		public enum FillType
		{
			Horizontal,
			Vertical,
			Both
		}

		public enum ShadowStyle
		{
			None,

			Light,
			Medium,
			Dark
		}

		public enum SizingHint
		{
			FixedSize,
            ShrinkToFit,
			FillAvailable
		}
		
		Point _Position;
		Size _Size;
		SizingHint _SizeHint;

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
		public ConsoleColor? ActiveBackground { get; set; }
		public ConsoleColor ActiveForeground { get; set; } = ConsoleColor.White;
		public ConsoleColor? SelectedBackground { get; set; }
		public ConsoleColor SelectedForeground { get; set; } = ConsoleColor.White;

		public ShadowStyle Shadow { get; set; }

		public object Tag { get; set; }
		public Control Parent { get; internal set; }

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

		public bool Enabled { get; set; }

		public bool IsFocused
		{
			get { return _HasFocus; }
			internal set
			{
				if (value != _HasFocus)
				{
					_HasFocus = value;

					if (_HasFocus && OnFocusGained != null)
						OnFocusGained(this, EventArgs.Empty);
					else if (!_HasFocus && OnFocusLost != null)
						OnFocusLost(this, EventArgs.Empty);
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
			if (IsFocusable)
				IsFocused = true;
			else if (Parent != null)
				Parent.Focus();
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

		internal virtual Point GetChildOffset(Control control)
		{
			if (Border == BorderStyle.None)
				return new Point(0, 0);
			return new Point(1, 1);
		}

		protected void Clear()
		{
			Graphics.DrawFilledBox(DisplayPosition, Size, ' ', Background ?? (Parent != null ? Parent.Background : null) ?? ConsoleColor.Black);
		}

		public void Draw(bool force = false)
		{
			if (!force && (!ShouldDraw || !_NeedsRedraw))
				return;

			_NeedsRedraw = false;

			lock (Console.Out)
			{
				Console.SetCursorPosition(DisplayPosition.X, DisplayPosition.Y);

				if (Border != BorderStyle.None)
				{
					var brush = BorderBrush;
					if (!Background.HasValue)
						brush = brush.Replace(' ', '\0');
					Graphics.DrawBox(DisplayPosition, Size, BorderBrush, Background ?? (Parent != null ? Parent.Background : null) ?? ConsoleColor.Black, Foreground);

					Console.SetCursorPosition(DisplayPosition.X + 1, DisplayPosition.Y + 1);
				}

				var oldBack = Console.BackgroundColor;
				var oldFront = Console.ForegroundColor;

				if (Background.HasValue)
				{
					Console.BackgroundColor = Background.Value;
					Point borderOffset = (Border == BorderStyle.None ? new Point(0, 0) : new Point(1, 1));
					Size borderSize = (Border == BorderStyle.None ? new Size(0, 0) : new Size(2, 2));

					if (Size >= borderSize)
						Graphics.DrawFilledBox(DisplayPosition + borderOffset, Size - borderSize, ' ', Background.Value);
				}
				else if (Parent != null && Parent.Background.HasValue)
					Console.BackgroundColor = Parent.Background.Value;

				Console.ForegroundColor = Foreground;
				
				if (Shadow != ShadowStyle.None)
				{
					Graphics.DrawLine(DisplayPosition + new Size(0, Size.Height + 1), DisplayPosition + Size + new Size(1, 1), Parent != null ? Parent.Background ?? ConsoleColor.Black : ConsoleColor.Black, Console.BackgroundColor, ShadowBrush);
					Graphics.DrawLine(DisplayPosition + new Size(Size.Width + 1, 0), DisplayPosition + Size + new Size(1, 1), Parent != null ? Parent.Background ?? ConsoleColor.Black : ConsoleColor.Black, Console.BackgroundColor, ShadowBrush);
				}

				Render();

				Console.BackgroundColor = oldBack;
				Console.ForegroundColor = oldFront;
			}
		}

		public abstract void Render();
	}
}

