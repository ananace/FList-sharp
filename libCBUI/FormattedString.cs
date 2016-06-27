using System;
using System.Collections;
using System.Collections.Generic;

namespace libCBUI
{
	public sealed class FormattedString : IComparable, ICloneable, IEnumerable, IComparable<string>
		, IEnumerable<char>, IEquatable<string>, IComparable<FormattedString>, IEquatable<FormattedString>
	{
		string _UnformattedCopy;
		string _ANSICopy;

		public string Unformatted { get { return _UnformattedCopy; } }

		public FormattedString()
		{
			
		}

		public FormattedString(string str)
		{
			_UnformattedCopy = str;
			_ANSICopy = str;
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
