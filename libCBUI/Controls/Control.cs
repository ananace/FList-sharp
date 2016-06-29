using System;
using System.Collections.Generic;
using System.Linq;

namespace libCBUI.Controls
{
	public abstract class Control : IControl
	{
		int _MinWidth;
		int _MaxWidth;
		int _Width;
		int _MinHeight;
		int _MaxHeight;
		int _Height;

		int validateWidth(int newWidth)
		{
			return newWidth < _MinWidth ? _MinWidth : newWidth > _MaxWidth ? _MaxWidth : newWidth;
		}
		int validateHeight(int newHeight)
		{
			return newHeight < _MinHeight ? _MinHeight : newHeight > _MaxHeight ? _MaxHeight : newHeight;
		}

		public Rect Bounds
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public IReadOnlyList<IControl> Children
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public Size DesiredSize
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public ConsoleColor EffectiveBackground
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public ConsoleColor EffectiveForeground
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool Focusable { get; set; }

		public bool IsEffectivelyEnabled
		{
			get
			{
				return IsEnabled && Parents.All(p => p.IsEnabled);
			}
		}

		public bool IsEffectivelyVisible
		{
			get
			{
				return IsVisible && Parents.All(p => p.IsVisible);
			}
		}

		public bool IsEnabled { get; set; }

		public bool IsFocused
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool IsVisible { get; set; }

		public Thickness Margin { get; set; }

		public int MaxHeight => _MaxHeight;
		public int MaxWidth => _MaxWidth;
		public int MinHeight => _MinHeight;
		public int MinWidth => _MinWidth;

		public int Height
		{
			get { return _Height; }
			set
			{
				_Height = validateHeight(value);
			}
		}
		public int Width
		{
			get { return _Width; }
			set
			{
				_Width = validateWidth(value);
			}
		}


		public string Name { get; set; }
		public IControl Parent { get; set; }

		public IEnumerable<IControl> Parents
		{
			get
			{
				IControl cur = this;
				while (cur != null)
				{
					yield return cur.Parent;
					cur = cur.Parent;
				}
			}
		}

		public HorizontalAlignment HorizontalAlignment { get; set; }
		public VerticalAlignment VerticalAlignment { get; set; }

		public event EventHandler OnFocusGained;
		public event EventHandler OnFocusLost;
		public event EventHandler<ConsoleKeyInfo> OnKeyInput;

		public void Focus()
		{
			if (!Focusable)
				return;

			throw new NotImplementedException();
		}

		public void InvalidateVisual()
		{
			throw new NotImplementedException();
		}

		public void Render()
		{
			throw new NotImplementedException();
		}
	}
}
