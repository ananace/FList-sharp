using libflist.Message;
using System;
using System.Windows;
using System.Windows.Documents;

namespace XAMLMessenger.Message
{
	[Node("date", Valid = NodeValidity.Internal)]
    class DateNode : ITextNode
    {
        public string Text { get; set; }

        DateTime Time { get
            {
                if (string.IsNullOrEmpty(Text))
                    return DateTime.Now;
                return DateTime.Parse(Text);
            }
        }

        public Inline ToInline(libflist.FChat.Channel _chan)
        {
            var time = Time;
            string timeString = null;
            if (time.Date == DateTime.Now.Date)
                timeString = time.ToShortTimeString();
            else
                timeString = time.ToShortDateString();

            return new Run($"[{timeString}]")
            {
                FontStyle = FontStyles.Italic
            };
        }
    }
}
