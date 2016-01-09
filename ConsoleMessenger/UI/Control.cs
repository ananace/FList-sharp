using System;
using System.Collections.Generic;
using ConsoleMessenger.Types;

namespace ConsoleMessenger.UI
{
	public abstract class Control
	{
		public enum FillType
		{
			Horizontal,
			Vertical,
			Both
		}

		Point _Offset;
		Point _Size;

		public Control Parent { get; set; }

		public Point Position { get; set; }
		public Point Size { get; set; }

		public virtual Point InternalPosition { get { return _Offset; } }
		public virtual Point InternalSize { get { return _Size; } }
		public virtual Point DrawPosition { get { return _Offset; } }
		public virtual Point DrawSize { get { return _Size; } }

		public virtual void Refresh()
		{
			Point totalSize = new Point(Console.BufferWidth - 1, Console.BufferHeight - 1);

			if (Parent != null)
			{
				_Offset = Point.Constrain(Parent.InternalPosition + Position, totalSize);
				_Size = Point.Constrain(Point.Constrain(Position + Size, Parent.InternalSize) - Position, totalSize);
			}
			else
			{
				_Offset = Point.Constrain(Position, totalSize);
				_Size = Point.Constrain(Size, totalSize);
			}
		}

		public virtual void Draw()
		{
			Console.SetCursorPosition(InternalPosition.X, InternalPosition.Y);
		}

		public virtual void Fill(FillType fill = FillType.Both)
		{
			Point minPosition;
			Point maxSize;

			if (Parent != null)
			{
				Parent.Refresh();

				minPosition = Parent.InternalPosition;
				maxSize = Parent.InternalSize;
			}
			else
			{
				minPosition = new Point(0, 0);
				maxSize = new Point(Console.BufferWidth - 1, Console.BufferHeight - 1);
			}

			if (fill == FillType.Both)
			{
				Position = minPosition;
				Size = maxSize;
			}
			else if (fill == FillType.Horizontal)
			{
				Position = new Point(minPosition.X, Position.Y);
				Size = new Point(maxSize.X, Size.Y);
			}
			else
			{
				Position = new Point(Position.X, minPosition.Y);
				Size = new Point(Size.X, maxSize.Y);
			}
		}
	}
}

