using System;
using System.Collections;
using System.Collections.Generic;
using ConsoleMessenger.Types;
using System.Linq;

namespace ConsoleMessenger.UI
{
	public class Panel : Control, IDisposable
	{
		class ChildCollection : IList<Control>, IDisposable
		{
			Panel _Owner;
			List<Control> _Children;

			internal ChildCollection(Panel own)
			{
				_Owner = own;
				_Children = new List<Control>();
			}

			public void Dispose()
			{
				_Children.Clear();
				_Children = null;
				_Owner = null;
			}

			public Control this[int index]
			{
				get
				{
					return _Children[index];
				}

				set
				{
					value.Parent = _Owner;
					_Children[index] = value;
				}
			}

			public int Count
			{
				get
				{
					return _Children.Count;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return ((IList<Control>)_Children).IsReadOnly;
				}
			}

			public void Add(Control item)
			{
				item.Parent = _Owner;
				_Children.Add(item);
			}

			public void Clear()
			{
				_Children.ForEach(c => c.Parent = null);
				_Children.Clear();
			}

			public bool Contains(Control item)
			{
				return _Children.Contains(item);
			}

			public void CopyTo(Control[] array, int arrayIndex)
			{
				_Children.CopyTo(array, arrayIndex);
			}

			public IEnumerator<Control> GetEnumerator()
			{
				return _Children.GetEnumerator();
			}

			public int IndexOf(Control item)
			{
				return _Children.IndexOf(item);
			}

			public void Insert(int index, Control item)
			{
				item.Parent = _Owner;
				_Children.Insert(index, item);
			}

			public bool Remove(Control item)
			{
				if (_Children.Remove(item))
				{
					item.Parent = null;
					return true;
				}

				return false;
			}

			public void RemoveAt(int index)
			{
				var item = _Children[index];
				item.Parent = null;
				_Children.Remove(item);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return _Children.GetEnumerator();
			}
		}

		Control _Child;
		ChildCollection _Children;
		
		public virtual Rect Padding { get; set; }
		public bool ResizeChildren { get; set; }

		protected virtual IList<Control> Children { get { return _Children; } }
		public virtual Control Child
		{
			get { return _Children.FirstOrDefault(); }
			set
			{
				if (value == _Child)
					return;
				
				_Children.Remove(_Child);
				if (value != null && !_Children.Contains(value))
					_Children.Add(value);

				_Child = value;
				InvalidateLayout();
			}
		}

		public Panel()
		{
			_Children = new ChildCollection(this);
		}

		public override void InvalidateLayout()
		{
			if (ResizeChildren)
				Child.Size = Size - (Size)Padding.Position - Padding.Size;

			InvalidateVisual();

			foreach (var child in _Children)
				child.InvalidateLayout();
		}

		internal override Point GetChildOffset(Control control)
		{
			return new Point(Padding.Left, Padding.Top);
		}

		public new void Dispose()
		{
			base.Dispose();

			_Children.Clear();
			_Children = null;
		}

		public override void Render()
		{
			if (_Child != null)
				_Child.Draw();
		}
	}
}

