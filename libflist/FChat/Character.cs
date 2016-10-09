using System;
using System.Linq;
using libflist.FChat.Commands;
using libflist.Info;
using libflist.Util;

namespace libflist.FChat
{

	public enum CharacterStatus
	{
		Offline,
		Online,
		Looking,
		Busy,
		DND,
		Idle,
		Away,

		[EnumValue("crown")]
		Rewarded
	}

	public enum TypingStatus
	{
		Clear,
		Typing,
		Paused
	}
	
	public sealed class Character : libflist.Character
	{
		public FChatConnection Connection { get; private set; }
		
		public Genders Gender { get; internal set; }
		public CharacterStatus Status { get; internal set; }
		public string StatusMessage { get; internal set; }
		
		public TypingStatus IsTyping { get; internal set; }
        public bool IsChatOp { get { return Connection.ChatOPs.Contains(this); } }

		internal Character(FChatConnection Connection, libflist.Character Character) : base(Character.Client, Character.Name)
		{
			this.Connection = Connection;

			_Description.UnderlyingTimestamp = Character._Description.UnderlyingTimestamp;
			_Description.UnderlyingValue = Character._Description.UnderlyingValue;
			_Images.UnderlyingTimestamp = Character._Images.UnderlyingTimestamp;
			_Images.UnderlyingValue = Character._Images.UnderlyingValue;
			_Kinks.UnderlyingTimestamp = Character._Kinks.UnderlyingTimestamp;
			_Kinks.UnderlyingValue = Character._Kinks.UnderlyingValue;
			_ProfileInfo.UnderlyingTimestamp = Character._ProfileInfo.UnderlyingTimestamp;
			_ProfileInfo.UnderlyingValue = Character._ProfileInfo.UnderlyingValue;
		}

		public void Dispose()
		{
			Name = null;
			Connection = null;
		}

		public bool IsOPInChannel(Channel c)
		{
			return c != null && c.OPs.Contains(this);
		}

		public void SendMessage(string message)
		{
			Connection.SendCommand(new Client_PRI_CharacterSendMessage {
				Character = Name,
				Message = message
			});
		}
	}
}

