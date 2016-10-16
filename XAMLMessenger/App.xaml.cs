using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using libflist;
using libflist.FChat;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Diagnostics;

namespace XAMLMessenger
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
        IFListClient _flist;
        FChatConnection _fchat;

		public event EventHandler<RequestNavigateEventArgs> RequestNavigate;

        public App()
        {
            _flist = new FListClientV1();
            _fchat = new FChatConnection(_flist);

            UriParser.Register(new FListUriParser(), "flist", 1);
        }


		public void OnRequestNavigate(object sender, RequestNavigateEventArgs ev)
		{
			var uri = ev.Uri;

			switch (uri.Scheme)
			{
				case "http":
				case "https":
					Process.Start(new ProcessStartInfo(uri.AbsoluteUri));
					ev.Handled = true;
					break;

				default:
					RequestNavigate?.Invoke(this, ev);
					break;
			}
		}

		public static new App Current { get { return Application.Current as App; } }
        public BitmapImage CombinedImageResource { get { return FindResource("CombinedImageResource") as BitmapImage; } }

        public IFListClient FListClient => _flist;
        public FChatConnection FChatClient => _fchat;
    }
}
