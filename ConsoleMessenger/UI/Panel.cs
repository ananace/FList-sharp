using System;
using System.Collections;
using System.Collections.Generic;
using ConsoleMessenger.Types;

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

		ChildCollection _Children;

		public virtual ConsoleColor BorderColor { get; set; }
		public virtual bool HasBorder { get; set; }
		public virtual int Padding { get; set; }

		public override Point InternalPosition { get {
				return base.InternalPosition + new Point(Padding + (HasBorder ? 1 : 0), Padding + (HasBorder ? 1 : 0));
			}
		}
		public override Point InternalSize { get {
				return base.InternalSize - new Point(Padding * 2 + (HasBorder ? 2 : 0), Padding * 2 + (HasBorder ? 2 : 0));
			}
		}

		public ICollection<Control> Children { get { return _Children; } }

		public Panel()
		{
			_Children = new ChildCollection(this);
		}

		public void Dispose()
		{
			_Children.Clear();
			_Children = null;
		}

		public override void Draw()
		{
			base.Draw();
			if (base.DrawSize.X <= 0 || base.DrawSize.Y <= 0)
				return;

			if (HasBorder)
				Graphics.DrawBox(base.DrawPosition, base.DrawSize, BorderColor);
			
			foreach (var child in _Children)
				child.Draw();
		}

		public override void Refresh()
		{
			base.Refresh();
		}
	}
}

