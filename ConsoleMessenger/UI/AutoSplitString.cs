using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ConsoleMessenger.Types;

namespace ConsoleMessenger.UI
{
    public class AutoSplitString
    {
        int _MaxLength;
        ANSIString[] _SplitString;
        ANSIString _String;

        public ANSIString[] SplitString => _SplitString;
        public ANSIString String
        {
            get { return _String; }
            set
            {
                if (_String.Equals(value))
                    return;

                _String = value;
                ResplitString();
            }
        }
        public int MaxLength
        {
            get { return _MaxLength; }
            set
            {
                if (_MaxLength == value)
                    return;

                _MaxLength = value;
                ResplitString();
            }
        }

        public AutoSplitString(ANSIString String)
        {
            _MaxLength = ConsoleHelper.Size.Width - 1;
            _String = String;

            ResplitString();
        }

        void ResplitString()
        {
            _SplitString = String
                .Split('\n')
                .SelectMany(s => {
                    if (s.Count <= _MaxLength)
                        return new[] { s };

                    var ret = new List<ANSIString>();
                    var splitS = s.Clone() as ANSIString;
                    while (splitS.Count > _MaxLength)
                    {
                        ret.Add(splitS.Substring(0, _MaxLength));
                        splitS = splitS.Remove(0, _MaxLength);
                    }

                    ret.Add(splitS);
                    return ret.ToArray();
                }).ToArray();
        }

        public ANSIString ToANSIString()
        {
            return ANSIString.Join("\n", _SplitString);
        }
        public override string ToString()
        {
            return ANSIString.Join("\n", _SplitString).PlainString;
        }
    }
}

