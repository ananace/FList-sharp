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
using libflist;
using libflist.FChat;
using System.Collections.ObjectModel;

namespace XAMLMessenger.Dialogs
{
	/// <summary>
	/// Interaction logic for CharacterDialog.xaml
	/// </summary>
	public partial class CharacterDialog : UserControl
	{
		public FChatConnection Client { get; private set; }

		public CharacterDialog(FChatConnection Client)
		{
			this.Client = Client;
            if (!Client.IsConnected)
                Client.Connect();

			InitializeComponent();

			_CharacterBox.ItemsSource = Client.FListClient.Ticket.Characters;
            _CharacterBox.SelectedIndex = 0;
		}

		private void _LoginButton_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(_CharacterBox.SelectedValue as string))
				return;

            Client.Login(_CharacterBox.SelectedValue as string);
		}

        private void _CharacterBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _LoginButton.IsEnabled = !string.IsNullOrEmpty(_CharacterBox.SelectedValue as string);
        }
    }
}
