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

namespace XAMLMessenger
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
        IFListClient _flist;
        FChatConnection _fchat;

        public App()
        {
            _flist = new FListClientV1();
            _fchat = new FChatConnection(_flist);
        }

        public static new App Current { get { return Application.Current as App; } }
        public BitmapImage StaticImageResource { get { return this.Resources["StaticImageResource"] as BitmapImage; } }

        public IFListClient FListClient => _flist;
        public FChatConnection FChatClient => _fchat;
    }
}
