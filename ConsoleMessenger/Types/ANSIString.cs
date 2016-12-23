using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleMessenger.Types
{
    public class ANSIString : IComparable, ICloneable, IConvertible, IList<ANSIString.ANSIChar>, IEquatable<ANSIString>,
        IEnumerable, IEnumerable<ANSIString.ANSIChar>, IComparable<ANSIString>, IComparable<string>, IEnumerable<char>, IEquatable<string>
    {
        public struct ANSIChar : IEquatable<ANSIChar>
        {
            public char UnicodeChar;
            public ConsoleColor? ForegroundColor;
            public ConsoleColor? BackgroundColor;
            // public bool Underline;

            public ANSIChar(char c)
            {
                UnicodeChar = c;
                ForegroundColor = null;
                BackgroundColor = null;
            }
            public ANSIChar(char c, ConsoleColor fg)
            {
                UnicodeChar = c;
                ForegroundColor = fg;
                BackgroundColor = null;
            }
            public ANSIChar(char c, ConsoleColor fg, ConsoleColor bg)
            {
                UnicodeChar = c;
                ForegroundColor = fg;
                BackgroundColor = bg;
            }
            public ANSIChar(ANSIChar c)
            {
                UnicodeChar = c.UnicodeChar;
                ForegroundColor = c.ForegroundColor;
                BackgroundColor = c.BackgroundColor;
            }

            public bool Equals(ANSIChar other)
            {
                return UnicodeChar == other.UnicodeChar &&
                       ForegroundColor == other.ForegroundColor &&
                       BackgroundColor == other.BackgroundColor;
            }
        }

        List<ANSIChar> _String;

        public string PlainString { get { return new string(_String.Select(c => c.UnicodeChar).ToArray()); } }

        public ConsoleColor? ForegroundColor
        {
            set
            {
                var str = new List<ANSIChar>(_String.Count);
                foreach (var c in _String)
                {
                    var ch = new ANSIChar(c);
                    ch.ForegroundColor = value;
                    str.Add(ch);
                }
                _String = str;
            }
        }

        public ConsoleColor? BackgroundColor
        {
            set
            {
                var str = new List<ANSIChar>(_String.Count);
                foreach (var c in _String)
                {
                    var ch = new ANSIChar(c);
                    ch.BackgroundColor = value;
                    str.Add(ch);
                }
                _String = str;
            }
        }


        public ANSIString()
        {
            _String = new List<ANSIChar>();
        }

        public ANSIString(string String)
        {
            _String = String.Select(c => new ANSIChar(c)).ToList();
        }

        public ANSIString(ANSIString String)
        {
            _String = String._String.Select(c => new ANSIChar(c)).ToList();
        }


        public void Append(string other)
        {
            _String.AddRange(new ANSIString(other));
        }

        public void Append(ANSIString other)
        {
            _String.AddRange(other._String);
        }


        public ANSIString Remove(int index, int length = -1)
        {
            var ret = new ANSIString();
            ret._String.AddRange(_String.Take(index));
            if (length >= 0)
                ret._String.AddRange(_String.Skip(index + length));

            return ret;
        }

        public ANSIString Replace(ANSIString search, ANSIString replace)
        {
            int index = 0;
            ANSIString ret = new ANSIString(this);
            do
            {
                index = PlainString.IndexOf(search.PlainString, index);
                if (index > 0)
                {
                    for (int i = 0; i < search.Count; ++i)
                    {
                        ret._String[index + i] = replace[i];
                    }
                }
            }
            while (index > 0);

            return ret;
        }

        public IEnumerable<ANSIString> Split(char separator)
        {
            int index = 0, newIndex = 0;
            var sep = new ANSIChar(separator);
            do
            {
                newIndex = _String.IndexOf(sep, index);
                if (newIndex < 0)
                {
                    var endstr = new ANSIString();
                    endstr._String.AddRange(_String.Skip(index));
                    yield return endstr;

                    break;
                }
                
                var first = index;
                index = _String.IndexOf(sep, index);

                var str = new ANSIString();
                str._String.AddRange(_String.Skip(first).Take(index < 0 ? _String.Count - first : index - first));
                yield return str;
            }
            while (index >= 0);
        }

        public ANSIString Substring(int index, int length = -1)
        {
            var ret = new ANSIString();
            ret._String.AddRange(_String.Skip(index).Take(length < 0 ? _String.Count - index : length));

            return ret;
        }

        public static ANSIString operator +(ANSIString a, ANSIString b)
        {
            var ret = new ANSIString(a);
            ret.Append(b);
            return ret;
        }
        public static ANSIString operator +(ANSIString a, string b)
        {
            var ret = new ANSIString(a);
            ret.Append(new ANSIString(b));
            return ret;
        }

        public static ANSIString Join(string sep, IEnumerable<ANSIString> parts)
        {
            return Join(new ANSIString(sep), parts);
        }
        public static ANSIString Join(ANSIString sep, IEnumerable<ANSIString> parts)
        {
            ANSIString ret = new ANSIString();

            foreach (var part in parts)
            {
                ret += part;
                if (!part.Equals(parts.Last()))
                    ret += sep;
            }

            return ret;
        }

        public string ToBBCode()
        {
            var _ColorLookup = new Dictionary<ConsoleColor,string>()
            {
                { ConsoleColor.Black, "black" },
                { ConsoleColor.Blue, "blue" },
                { ConsoleColor.Cyan, "cyan" },
                { ConsoleColor.DarkBlue, "blue" },
                { ConsoleColor.DarkCyan, "cyan" },
                { ConsoleColor.DarkGray, "gray" },
                { ConsoleColor.DarkGreen, "green" },
                { ConsoleColor.DarkMagenta, "purple" },
                { ConsoleColor.DarkRed, "brown" },
                { ConsoleColor.DarkYellow, "orange" },
                { ConsoleColor.Gray, "gray" },
                { ConsoleColor.Green, "green" },
                { ConsoleColor.Magenta, "pink" },
                { ConsoleColor.Red, "red" },
                { ConsoleColor.White, "white" },
                { ConsoleColor.Yellow, "yellow" },
            };

            var build = new StringBuilder();

            ConsoleColor? oldFore = null;
            foreach (var c in _String)
            {
                if (c.ForegroundColor != oldFore)
                {
                    if (oldFore.HasValue)
                        build.Append("[/color]");

                    oldFore = c.ForegroundColor;
                    if (oldFore.HasValue)
                        build.Append($"[color={ _ColorLookup[oldFore.Value] }]");
                }

                build.Append(c.UnicodeChar);
            }

            if (oldFore.HasValue)
                build.Append("[/color]");

            return build.ToString();
        }

        #region Interfaces

        public int Count
        {
            get
            {
                return ((IReadOnlyList<ANSIChar>)_String).Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((IList<ANSIChar>)_String).IsReadOnly;
            }
        }

        ANSIChar IList<ANSIChar>.this[int index]
        {
            get
            {
                return ((IList<ANSIChar>)_String)[index];
            }

            set
            {
                ((IList<ANSIChar>)_String)[index] = value;
            }
        }

        public ANSIChar this[int index]
        {
            get
            {
                return ((IReadOnlyList<ANSIChar>)_String)[index];
            }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)_String).GetEnumerator();
        }

        public int CompareTo(string other)
        {
            string toCompare = PlainString;
            return toCompare.CompareTo(other);
        }
        public int CompareTo(ANSIString other)
        {
            string toCompare = PlainString;
            return toCompare.CompareTo(other.PlainString);
        }

        public int CompareTo(object obj)
        {
            if (obj is string)
                return CompareTo((string)obj);
            else if (obj is ANSIString)
                return CompareTo((ANSIString)obj);
            throw new NotImplementedException();
        }

        IEnumerator<char> IEnumerable<char>.GetEnumerator()
        {
            return PlainString.GetEnumerator();
        }

        public bool Equals(ANSIString other)
        {
            if (Count == other.Count)
            {
                for (int i = 0; i < Count; ++i)
                {
                    if (!this[i].Equals(other[i]))
                        return false;
                }

                return true;
            }

            return false;
        }

        public bool Equals(string other)
        {
            string toCompare = PlainString;
            return toCompare.Equals(other);
        }

        public object Clone()
        {
            return new ANSIString(this);
        }

        public TypeCode GetTypeCode()
        {
            return PlainString.GetTypeCode();
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return ((IConvertible)PlainString).ToBoolean(provider);
        }

        public char ToChar(IFormatProvider provider)
        {
            return ((IConvertible)PlainString).ToChar(provider);
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return ((IConvertible)PlainString).ToSByte(provider);
        }

        public byte ToByte(IFormatProvider provider)
        {
            return ((IConvertible)PlainString).ToByte(provider);
        }

        public short ToInt16(IFormatProvider provider)
        {
            return ((IConvertible)PlainString).ToInt16(provider);
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return ((IConvertible)PlainString).ToUInt16(provider);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return ((IConvertible)PlainString).ToInt32(provider);
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return ((IConvertible)PlainString).ToUInt32(provider);
        }

        public long ToInt64(IFormatProvider provider)
        {
            return ((IConvertible)PlainString).ToInt64(provider);
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return ((IConvertible)PlainString).ToUInt64(provider);
        }

        public float ToSingle(IFormatProvider provider)
        {
            return ((IConvertible)PlainString).ToSingle(provider);
        }

        public double ToDouble(IFormatProvider provider)
        {
            return ((IConvertible)PlainString).ToDouble(provider);
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return ((IConvertible)PlainString).ToDecimal(provider);
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return ((IConvertible)PlainString).ToDateTime(provider);
        }

        public string ToString(IFormatProvider provider)
        {
            return PlainString.ToString(provider);
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return ((IConvertible)PlainString).ToType(conversionType, provider);
        }

        IEnumerator<ANSIChar> IEnumerable<ANSIChar>.GetEnumerator()
        {
            return ((IEnumerable<ANSIChar>)_String).GetEnumerator();
        }

        public int IndexOf(ANSIChar item)
        {
            return ((IList<ANSIChar>)_String).IndexOf(item);
        }

        public void Insert(int index, ANSIChar item)
        {
            ((IList<ANSIChar>)_String).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<ANSIChar>)_String).RemoveAt(index);
        }

        public void Add(ANSIChar item)
        {
            ((IList<ANSIChar>)_String).Add(item);
        }

        public void Clear()
        {
            ((IList<ANSIChar>)_String).Clear();
        }

        public bool Contains(ANSIChar item)
        {
            return ((IList<ANSIChar>)_String).Contains(item);
        }

        public void CopyTo(ANSIChar[] array, int arrayIndex)
        {
            ((IList<ANSIChar>)_String).CopyTo(array, arrayIndex);
        }

        public bool Remove(ANSIChar item)
        {
            return ((IList<ANSIChar>)_String).Remove(item);
        }

        #endregion
    }
}

