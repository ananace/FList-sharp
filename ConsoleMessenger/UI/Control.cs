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

		Point _Position;
		Size _Size;

		bool _HasFocus;
		bool _NeedsRedraw = true;
		bool _Visible = true;

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
		public ConsoleColor Foreground { get; set; }
		public ConsoleColor? ActiveBackground { get; set; }
		public ConsoleColor ActiveForeground { get; set; }
		public ConsoleColor? SelectedBackground { get; set; }
		public ConsoleColor SelectedForeground { get; set; }

		public ShadowStyle Shadow { get; set; }

		public Control Parent { get; internal set; }

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

		public bool Enabled { get; set; }
		
		public bool IsFocused { get { return _HasFocus; }
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

		public bool IsVisible {
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

		internal virtual bool ShouldDraw { get { return _Visible && (Parent == null || Parent.ShouldDraw); } }
		
		public virtual Point DisplayPosition
		{
			get
			{
				return Parent == null ? Position : Parent.DisplayPosition + Parent.GetChildOffset(this) + Position;
			}
		}

		public event EventHandler OnVisualInvalidated;
		public event EventHandler OnFocusGained;
		public event EventHandler OnFocusLost;

		public void Dispose()
		{
			Parent = null;
		}

		public virtual void InvalidateLayout()
		{
			InvalidateVisual();
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

		public void Draw()
		{
			if (!ShouldDraw || !_NeedsRedraw)
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

				Render();
			}
		}

		public abstract void Render();
	}
}

