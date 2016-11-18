using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleMessenger.UI
{
    public class AutoSplitString
    {
        int _MaxLength;
        string[] _SplitString;
        string _String;

        public string[] SplitString => _SplitString;
        public string String
        {
            get { return _String; }
            set
            {
                if (_String == value)
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

        public AutoSplitString(string String)
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
                    if (s.ANSILength() <= _MaxLength)
                        return new[] { s };

                    var ret = new List<string>();
                    var splitS = s.Clone() as string;
                    while (splitS.Length > _MaxLength)
                    {
                        ret.Add(splitS.Substring(0, _MaxLength));
                        splitS = splitS.Remove(0, _MaxLength);
                    }

                    ret.Add(splitS);
                    return ret.ToArray();
                }).ToArray();
        }

        public override string ToString()
        {
            return string.Join("\n", _SplitString);
        }
    }
}

