using System;
using System.Linq;

namespace ConsoleMessenger.UI.FChat
{
	class StatusBar : ContentControl
	{
		libflist.FChat _Chat;
		public libflist.FChat Chat
		{
			get { return _Chat; }
			set
			{
				if (_Chat == value) return;
				if (_Chat != null)
				{
					_Chat.Connection.OnIdentified -= OnEvent;
					_Chat.OnJoinChannel -= OnEvent;
					_Chat.OnLeaveChannel -= OnEvent;
				}

				_Chat = value;

				_Chat.Connection.OnIdentified += OnEvent;
				_Chat.OnJoinChannel += OnEvent;
				_Chat.OnLeaveChannel += OnEvent;
			}
        }


		public override void Render()
		{
			Console.ForegroundColor = ConsoleColor.DarkCyan;
			Console.Write("[");
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write(DateTime.Now.ToShortTimeString());
			Console.ForegroundColor = ConsoleColor.DarkCyan;
			Console.Write("]");

			Console.CursorLeft++;

			Graphics.WriteANSIString(ContentDrawable);
		}

		private void OnEvent(object a, object b)
		{
			RebuildString();
		}
		private void RebuildString()
		{
			var oB = "[".Color(ConsoleColor.DarkCyan);
			var cB = "]".Color(ConsoleColor.DarkCyan) + " ";

			int i = 0;
			Content = string.Format("{0}{1}{2}",
				_Chat.LocalCharacter != null ?
					oB + _Chat.LocalCharacter.Name.Color(_Chat.LocalCharacter.GenderColor) + cB:
					"",
				oB /* TODO <cur chan number>:<chan name> */ + cB,
				oB + "Act: " + string.Join(",",_Chat.JoinedChannels.Select(c => i++)) + cB // FIXME Actually show activity, not just buffers
				);
		}
	}
}
