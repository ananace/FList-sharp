﻿using System;
using System.Linq;
using ConsoleMessenger.Types;

namespace ConsoleMessenger.UI.FChat
{
	class StatusBar : ContentControl
	{
		libflist.FChat.FChatConnection _Chat;
		public libflist.FChat.FChatConnection Chat
		{
			get { return _Chat; }
			set
			{
				if (_Chat == value) return;
				if (_Chat != null)
				{
					_Chat.OnIdentified -= OnEvent;
					_Chat.OnChannelJoin -= OnEvent;
					_Chat.OnChannelLeave -= OnEvent;
				}

				_Chat = value;

				_Chat.OnIdentified += OnEvent;
				_Chat.OnChannelJoin += OnEvent;
				_Chat.OnChannelLeave += OnEvent;
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
					oB + _Chat.LocalCharacter.Name.Color(_Chat.LocalCharacter.GetGenderColor()) + cB:
					"",
				oB /* TODO <cur chan number>:<chan name> */ + cB,
				oB + "Act: " + string.Join(",",_Chat.ActiveChannels.Select(c => i++)) + cB // FIXME Actually show activity, not just buffers
				);
		}
	}
}
