using System;
using System.Collections.Generic;
using System.Linq;

namespace libCBUI.Controls
{
	public abstract class Control : InputElement, IControl
	{
		int _MinWidth;
		int _MaxWidth;
		int _Width;
		int _MinHeight;
		int _MaxHeight;
		int _Height;
		bool _Focused;

		ControlCollection _Children = new ControlCollection();

		int validateWidth(int newWidth)
		{
			return newWidth < _MinWidth ? _MinWidth : newWidth > _MaxWidth ? _MaxWidth : newWidth;
		}
		int validateHeight(int newHeight)
		{
			return newHeight < _MinHeight ? _MinHeight : newHeight > _MaxHeight ? _MaxHeight : newHeight;
		}

		public Rect EffectiveBounds
		{
			get
			{
				var bound = Bounds;

				if (this is IStyled)
					bound -= (this as IStyled).Padding;

				return bound;
			}
		}

		public Rect Bounds
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public ControlCollection Children
		{
			get
			{
				return _Children;
			}
		}

		public Size DesiredSize { get; set; }
		public bool IsVisible { get; set; }

		public Thickness Margin { get; set; }

		// TODO: Add validation
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


		public void InvalidateVisual()
		{
			throw new NotImplementedException();
		}

		public virtual void Draw()
		{
			using (new ColorChanger(this.GetEffectiveBackground(), this.GetEffectiveForeground()))
			using (new CursorChanger(EffectiveBounds.Position, false))
			{
				if (this is IStyled)
				{
					var vis = this as IStyled;

					if (vis.Background.HasValue)
					{
						Graphics.DrawFilledBox(Bounds.Position, Bounds.Size, ' ', vis.Background.Value);
					}
					if (vis.Border != BorderStyle.None)
					{
						Graphics.DrawBox(Bounds.Position, Bounds.Size, vis.GetBorderBrush());
					}
				}

				Render();
			}
		}
		public abstract void Render();

	}
}
