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

namespace XAMLMessenger.Dialogs
{
	/// <summary>
	/// Interaction logic for LoginDialog.xaml
	/// </summary>
	public partial class LoginDialog : UserControl
	{
		public event EventHandler OnLogin;

		public IFListClient Client { get; private set; }

		public LoginDialog(IFListClient Client)
		{
			InitializeComponent();

			this.Client = Client;
		}

		private void _LoginButton_Click(object sender, RoutedEventArgs e)
		{
			_LoginButton.IsEnabled = false;
            _PasswordBox.IsEnabled = false;
            _UsernameBox.IsEnabled = false;
            
			Client.Authenticate(_UsernameBox.Text, _PasswordBox.Password).ContinueWith(task =>
			{
				if (task.IsCompleted && task.Result)
					OnLogin?.Invoke(this, EventArgs.Empty);
				else
				{
					_LoginButton.IsEnabled = true;
                    _PasswordBox.IsEnabled = true;
                    _UsernameBox.IsEnabled = true;
                    _PasswordBox.Password = "";
				}
			});
		}
	}
}
