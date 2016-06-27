using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleMessenger.Types;

namespace ConsoleMessenger.UI
{
	public class TextArea : Control
	{
		int _Offset;
		string _Text = "";
		List<string> _TextSplits = new List<string>();

		public bool InputEnabled { get; set; }
		public event EventHandler<string> OnTextChanged;

		public string Text
		{
			get { return _Text; }
			set
			{
				OnTextChanged?.Invoke(this, value);

				_Text = value ?? "";
				updateSplits();
			}
		}

		public int Offset
		{
			get { return _Offset; }
			set
			{
				_Offset = value;

				if (_Offset < 0)
					_Offset = 0;
				else if (_TextSplits.Count > 0 && _Offset >= _TextSplits.Count)
					_Offset = _TextSplits.Count - 1;
				else
					_Offset = 0;
			}
		}

		public TextArea()
		{
		}

		void updateSplits()
		{
			_TextSplits.Clear();

			var lastSplit = 0;
			for (var i = 0; i < _Text.Length; ++i)
			{
				if (_Text[i] == '\n' || i - lastSplit >= Size.Width)
				{
					_TextSplits.Add(_Text.Substring(lastSplit, i - lastSplit));
					lastSplit = i + (_Text[i] == '\n' ? 1 : 0);
				}
			}

			if (lastSplit < _Text.Length)
				_TextSplits.Add(_Text.Substring(lastSplit, _Text.Length - lastSplit));
		}


		internal override bool IsFocusable { get { return InputEnabled; } }

		public override void InvalidateLayout()
		{
			base.InvalidateLayout();
			updateSplits();
		}

		public override void Render()
		{
			if (!_TextSplits.Any())
				return;

			for (int i = _Offset; i < _TextSplits.Count && i < Size.Height - _Offset; ++i)
				Graphics.WriteANSIString(_TextSplits[i], Position + new Point(0, i - _Offset));

			// TODO: Draw scroll?
		}
	}
}
