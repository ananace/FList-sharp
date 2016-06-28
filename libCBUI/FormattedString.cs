using System;
using System.Collections;
using System.Collections.Generic;

namespace libCBUI
{

	public sealed class FormattedString : IComparable, ICloneable, IEnumerable, IComparable<string>
		, IEnumerable<char>, IEquatable<string>, IComparable<FormattedString>, IEquatable<FormattedString>
		, IReadOnlyList<char>
	{
		readonly string _UnformattedCopy;
		readonly string _ANSICopy;

		public string Unformatted { get { return _UnformattedCopy; } }
		public string ANSIFormatted { get { return _ANSICopy; } }

		public int Count { get { return _UnformattedCopy.Length; } }
		public char this[int index] { get { return _UnformattedCopy[index]; } }

		public FormattedString()
		{
			
		}

		public FormattedString(string rawStr)
		{
			_UnformattedCopy = rawStr;
			_ANSICopy = rawStr;
		}

		public FormattedString(string str, ConsoleColor color)
		{
			_UnformattedCopy = str;
			_ANSICopy = str.Color(color);
		}

		public int CompareTo(object obj)
		{
			if (obj is FormattedString)
				return CompareTo(obj as FormattedString);
			if (obj is string)
				return CompareTo(obj as string);
			return 0;
		}

		public object Clone()
		{
			return new FormattedString(_ANSICopy);
		}

		public IEnumerator GetEnumerator()
		{
			return _UnformattedCopy.GetEnumerator();
		}

		public int CompareTo(string other)
		{
			return string.Compare(_UnformattedCopy, other, StringComparison.Ordinal);
		}

		IEnumerator<char> IEnumerable<char>.GetEnumerator()
		{
			return ((IEnumerable<char>)_UnformattedCopy).GetEnumerator();
		}

		public bool Equals(string other)
		{
			return _UnformattedCopy.Equals(other);
		}

		public int CompareTo(FormattedString other)
		{
			return string.Compare(_UnformattedCopy, other._UnformattedCopy, StringComparison.Ordinal);
		}

		public bool Equals(FormattedString other)
		{
			return string.Equals(_ANSICopy, other._ANSICopy, StringComparison.Ordinal);
		}
	}
}
