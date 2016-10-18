using System;
using System.Collections.Generic;
using libflist.FChat;
using System.Linq;
using System.Diagnostics;

namespace ConsoleMessenger
{
	public enum MessageType
	{
		Chat,
		LFRP,
		Roll,

        Preview
	}
    public enum MessageSource
    {
        Local,
        Remote
    }

	public class ChatBuffer
	{
		public class MessageData
		{
			public DateTime Timestamp { get; set; }
			public string Message { get; set; }
            public int Lines { get; set; }
			public Character Sender { get; set; }
			public MessageType Type { get; set; }
            public MessageSource Source { get; set; }
		}

        public event EventHandler<MessageData> OnMessage;

		List<MessageData> _Messages = new List<MessageData>();
		public IReadOnlyList<MessageData> Messages { get { return _Messages; } }
        public int MaxMessages { get; set; } = 1000;

		public void Clear()
		{
			_Messages.Clear();
		}

        public void PushMessage(Character sender, string message, MessageType type = MessageType.Chat, MessageSource source = MessageSource.Local, DateTime? timestamp = null)
		{
			_Messages.Add(new MessageData {
				Timestamp = timestamp ?? DateTime.Now,
				Sender = sender,
				Message = message,
				Type = type,
                Source = source
			});

            while (_Messages.Count >= MaxMessages)
                _Messages.RemoveAt(0);

            OnMessage?.Invoke(this, _Messages.Last());
		}

		public virtual void SendMessage(Character sender, string message, MessageType type = MessageType.Chat, MessageSource source = MessageSource.Local)
		{
			PushMessage(sender, message);
		}
	}

    public sealed class ConsoleChatBuffer : ChatBuffer
    {
		class DebugTracer : TraceListener
		{
			ConsoleChatBuffer _Buf;
			public DebugTracer(ConsoleChatBuffer buf)
			{
				_Buf = buf;
			}

			string partial;

			public override void Write(string message)
			{
				if (partial == null)
					partial = message;
				else
					partial += message;

				if (partial.Contains("\n"))
				{
					_Buf.SendMessage(null, partial.Substring(0, partial.IndexOf('\n')));
					partial = partial.Remove(0, partial.IndexOf('\n') + 1);
				}
			}

			public override void WriteLine(string message)
			{
				_Buf.SendMessage(null, message);
			}
		}

        DebugTracer _Tracer;
		public TraceListener TraceListener { get { if (_Tracer == null) _Tracer = new DebugTracer(this); return _Tracer; } }
        
    }

	public sealed class ChannelChatBuffer : ChatBuffer
	{
		public Channel Channel { get; private set; }

		public ChannelChatBuffer(Channel channel)
		{
			Channel = channel;
		}

		public override void SendMessage(Character sender, string message, MessageType type = MessageType.Chat, MessageSource source = MessageSource.Local)
		{
			if (source == MessageSource.Local)
				switch (type)
				{
					case MessageType.Chat:
						Channel.SendMessage(message); break;
					case MessageType.LFRP:
						Channel.SendLFRP(message); break;
					case MessageType.Roll:
                        Channel.SendRoll(message); return;
				}

			PushMessage(sender, message, type, source);
		}
	}

	public sealed class CharacterChatBuffer : ChatBuffer
	{
		public Character Character { get; private set; }

		public CharacterChatBuffer(Character character)
		{
			Character = character;
		}

		public override void SendMessage(Character sender, string message, MessageType type = MessageType.Chat, MessageSource source = MessageSource.Local)
		{
			if (source == MessageSource.Local && type == MessageType.Chat)
				Character.SendMessage(message);
			PushMessage(sender, message, type, source);
		}
	}
}

