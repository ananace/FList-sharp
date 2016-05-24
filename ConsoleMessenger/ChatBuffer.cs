using System;
using System.Collections.Generic;
using libflist.FChat;

namespace ConsoleMessenger
{
	public class ChatBuffer
	{
		public class MessageData
		{
			public DateTime Timestamp { get; set; }
			public string Message { get; set; }
			public Character Sender { get; set; }
		}

		List<MessageData> _Messages = new List<MessageData>();
		public IReadOnlyList<MessageData> Messages { get { return _Messages; } }

		public void PushMessage(Character sender, string message)
		{
			_Messages.Add(new MessageData {
				Timestamp = DateTime.Now,
				Sender = sender,
				Message = message
			});
		}

		public virtual void SendMessage(Character sender, string message)
		{
			PushMessage(sender, message);
		}
	}

	public sealed class ChannelChatBuffer : ChatBuffer
	{
		public Channel Channel { get; private set; }

		public ChannelChatBuffer(Channel channel)
		{
			Channel = channel;
		}

		public override void SendMessage(Character sender, string message)
		{
			Channel.SendMessage(message);
			PushMessage(sender, message);
		}
	}

	public sealed class CharacterChatBuffer : ChatBuffer
	{
		public Character Character { get; private set; }

		public CharacterChatBuffer(Character character)
		{
			Character = character;
		}

		public override void SendMessage(Character sender, string message)
		{
			Character.SendMessage(message);
			PushMessage(sender, message);
		}
	}
}

