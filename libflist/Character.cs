using System;
using System.Linq;
using libflist.Connection.Commands;
using libflist.Connection.Types;

namespace libflist
{
	public class Character : IDisposable
	{
		public FChat Chat { get; private set; }

		public string Name { get; private set; }
		public CharacterGender Gender { get; internal set; }
		public CharacterStatus Status { get; internal set; }
		public string StatusMessage { get; internal set; }

		public bool IsDisposed { get; private set; }
		public bool IsTyping { get; internal set; }

		internal Character(FChat Chat, string Name)
		{
			this.Chat = Chat;
			this.Name = Name;
		}

		public void Dispose()
		{
			Name = null;
			Chat = null;

			IsDisposed = true;
		}

		public bool IsOPInChannel(Channel c)
		{
			return c.OPs.Contains(this);
		}

		public void SendMessage(string message)
		{

		}

		public void SendCommand(Command cmd)
		{
			Chat.SendCommand(cmd);	
		}
	}
}

