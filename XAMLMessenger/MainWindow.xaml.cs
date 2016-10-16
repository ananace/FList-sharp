using libflist.FChat;
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
        ChatChannel GetOrCreateChannel(Channel channel)
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
                Header = new ChatTab(channel),
                Content = new ChatChannel(channel),
                Tag = channel
            };
            _chatList.Items.Add(item);

            return item.Content as ChatChannel;
        }
        TabItem GetTabByChannel(Channel channel)
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

		public FChatConnection Connection => App.Current.FChatClient;

		public MainWindow()
		{
			InitializeComponent();

#if false
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

			Connection.OnOfficialListUpdate += (s, e) =>
			{
				_PublicChannels.Items.Clear();
				foreach (var chan in Connection.OfficialChannels)
					_PublicChannels.Items.Add(new ListBoxItem
					{
						Content = chan.Title,
						Tag = chan
					});
			};
			Connection.OnPrivateListUpdate += (s, e) =>
			{
				_PrivateRooms.Items.Clear();
				foreach (var chan in Connection.PrivateChannels)
					_PrivateRooms.Items.Add(new ListBoxItem
					{
						Content = chan.Title,
						Tag = chan
					});

			};
#endif

			App.Current.RequestNavigate += (s, e) =>
			{
				if (e.Uri.Scheme != "flist")
					return;

				switch (e.Uri.Host)
				{
					case "session":
						{
							var chan = Connection.GetOrJoinChannel(e.Uri.Segments.Last());
							var tab = GetOrCreateChannel(chan);

							_chatList.SelectedItem = tab;
						} break;
				}
			};
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            _consoleChat.AddMessage("[session]Cuntboy[/session] [url=https://google.com][icon]ananace[/icon] or [i][sub]google[/sub][/i][/url]");
            _consoleChat.AddMessage("[session]Cuntboy[/session] [url=https://google.com][icon]ananace[/icon] or [i][sub]google[/sub][/i][/url]");
            _consoleChat.AddMessage("[session]Cuntboy[/session] [url=https://google.com][icon]ananace[/icon] or [i][sub]google[/sub][/i][/url]");
            _consoleChat.AddMessage("[session]Cuntboy[/session] [icon]ananace[/icon][url=https://google.com] or [i][sub]google[/sub][/i][/url]");
        }
    }
}
