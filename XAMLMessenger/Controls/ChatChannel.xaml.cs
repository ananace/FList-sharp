using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using libflist.FChat;
using libflist.Info;
using System.Globalization;
using XAMLMessenger.Message.Nodes;
using XAMLMessenger.Message;

namespace XAMLMessenger.Controls
{
    /// <summary>
    /// Interaction logic for ChatChannel.xaml
    /// </summary>
    public partial class ChatChannel : UserControl
    {
        Channel _channel;
        public IReadOnlyCollection<libflist.Character> Users => _channel.Characters;
        public Channel Channel => _channel;

        public ChatChannel() : this(null)
        {
        }

        public ChatChannel(Channel chan)
        {
            _channel = chan;

            InitializeComponent();
        }

        void baseMessage(Paragraph par, Character sender, string message)
        {
            if (sender == App.Current.FChatClient.LocalCharacter)
                par.Background = new SolidColorBrush
                {
                    Color = Colors.Black,
                    Opacity = 0.1
                };

            par.Inlines.AddRange(new Inline[]{
                new DateNode().ToInline(_channel),
                new CharacterNode { Text = sender.Name }.ToInline(_channel)
            });

            par.Inlines.AddRange(new Parser().ParseMessage(message).Select(n => n.ToInline(_channel)));
        }

        public void AddAction(Character sender, string action)
        {
            var par = new Paragraph();
            par.FontStyle = FontStyles.Italic;
            par.Foreground = Brushes.White;

            baseMessage(par, sender, action);

            _messageList.Document.Blocks.Add(par);
        }
        public void AddMessage(Character sender, string message)
        {
            var par = new Paragraph();
            par.Foreground = Brushes.White;

            baseMessage(par, sender, $": {message}");

            _messageList.Document.Blocks.Add(par);
        }
        public void AddSYSMessage(string message)
        {
            _messageList.AddMessage($"System: {message}");
        }
        public void AddLFRPMessage(Character sender, string message)
        {
            var par = new Paragraph();
            par.Foreground = Brushes.White;
            par.Background = Brushes.DarkGreen;

            baseMessage(par, sender, $": {message}");

            _messageList.Document.Blocks.Add(par);
        }
        
        static Brush GetBrush(Genders gender)
        {
            switch (gender)
            {
                case Genders.Cuntboy:
                    return Brushes.Green;
            }

            return Brushes.White;
        }

        static Brush GetBrush(CharacterStatus status)
        {
            switch (status)
            {
                case CharacterStatus.Online:
                    return Brushes.Gray;
                case CharacterStatus.Looking:
                    return Brushes.Green;
            }

            return Brushes.DarkGray;
        }
    }

    public class HalfConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
              object parameter, CultureInfo culture)
        {
            return (double)value / 2;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return (double)value * 2;
        }
    }
}
