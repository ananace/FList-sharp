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
using System.Windows.Shapes;

namespace XAMLMessenger
{
	/// <summary>
	/// Interaction logic for LoginWindow.xaml
	/// </summary>
	public partial class LoginWindow : Window
	{
		public object CurrentDialog
		{ get; private set; }

		public LoginWindow()
		{
            App.Current.FChatClient.OnIdentified += (s, e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    new MainWindow().Show();
                    Close();
                });
            };

			var login = new Dialogs.LoginDialog(App.Current.FListClient);
			login.OnLogin += (_, __) =>
			{
				Dispatcher.Invoke(() => {
					CurrentDialog = new Dialogs.CharacterDialog(App.Current.FChatClient);
					_Content.GetBindingExpression(ContentControl.ContentProperty).UpdateTarget();
				});
			};

			CurrentDialog = login;

			InitializeComponent();
			DataContext = this;
		}
	}
}
