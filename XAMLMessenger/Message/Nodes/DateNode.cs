using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace XAMLMessenger.Message.Nodes
{
    class DateNode : ITextNode
    {
        public string Name { get; } = "date";
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
