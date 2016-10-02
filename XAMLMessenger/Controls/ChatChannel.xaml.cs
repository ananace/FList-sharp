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

        public ChatChannel(Channel chan)
        {
            _channel = chan;

            InitializeComponent();
        }

        void baseMessage(Paragraph par, Character sender, string message)
        {
            par.Inlines.Add(new Run()
            {
                Text = DateTime.Now.ToShortTimeString(),
                Foreground = Brushes.White,
                FontStyle = FontStyles.Italic 
            });
            par.Inlines.Add(new Run()
            {
                Text = "¤",
                Foreground = GetBrush(sender.Status),
                FontStyle = FontStyles.Normal
            });
            par.Inlines.Add(new Run()
            {
                Text = sender.Name,
                Foreground = GetBrush(sender.Gender),
            });
            par.Inlines.AddRange(_messageList.ParseMessage(message));
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
            _messageList.AddMessage(message);
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
}
