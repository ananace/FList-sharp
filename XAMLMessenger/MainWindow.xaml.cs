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
using XAMLMessenger.Controls;

namespace XAMLMessenger
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
        ChatChannel GetOrCreateChannel(libflist.FChat.Channel channel)
        {
            foreach (var tab in _chatList.Items)
            {
                if ((tab as TabItem).Tag == channel)
                {
                    return (tab as TabItem).Content as ChatChannel;
                }
            }

            var item = new TabItem()
            {
                Header = new BitmapImage(new Uri($"pack://siteoforigin:,,,/Resources/{(channel.Official ? "hash" : "key")}.png")),
                Content = new ChatChannel(channel),
                Tag = channel
            };
            _chatList.Items.Add(item);

            return item.Content as ChatChannel;
        }
        TabItem GetTabByChannel(libflist.FChat.Channel channel)
        {
            foreach (var tab in _chatList.Items)
            {
                if ((tab as TabItem).Tag == channel)
                {
                    return tab as TabItem;
                }
            }
            return null;
        }

		public MainWindow()
		{
			InitializeComponent();

            App.Current.FChatClient.OnSYSMessage += (s, e) =>
            {
                _consoleChat.AddMessage((e.Command as libflist.FChat.Commands.Server_SYS_ChatSYSMessage).Message);
            };

            App.Current.FChatClient.OnChannelJoin += (s, e) =>
            {
                GetOrCreateChannel(e.Channel);
            };
            App.Current.FChatClient.OnChannelLeave += (s, e) =>
            {
                _chatList.Items.Remove(GetTabByChannel(e.Channel));
            };

            App.Current.FChatClient.OnChannelChatMessage += (s, e) =>
            {
                if (e.Message.StartsWith("/me ") || e.Message.StartsWith("/em "))
                    GetOrCreateChannel(e.Channel).AddAction(e.Character, e.Message.Substring(4));
                else
                    GetOrCreateChannel(e.Channel).AddMessage(e.Character, e.Message);
            };
            App.Current.FChatClient.OnChannelSYSMessage += (s, e) =>
            {
                GetOrCreateChannel(e.Channel).AddSYSMessage(e.Data);
                _consoleChat.AddMessage(e.Data);
            };
            App.Current.FChatClient.OnChannelLFRPMessage += (s, e) =>
            {
                GetOrCreateChannel(e.Channel).AddLFRPMessage(e.Character, e.Message);
            };
        }
	}
}
