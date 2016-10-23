using libflist.FChat;
using libflist.Message;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ConsoleMessenger.Logging
{
	class MessageSerializer
	{
		static readonly Dictionary<MessageType, string> _TypeDict = new Dictionary<MessageType, string>
		{
			{ MessageType.Chat, "MSG" },
			{ MessageType.LFRP, "LRP" },
			{ MessageType.Roll, "RLL" }
		};

		public FChatConnection Chat { get; private set; }

		public MessageSerializer(FChatConnection chat)
		{
			Chat = chat;
		}

		public void Serialize(Stream stream, ChatBuffer.MessageData message)
		{
			if (message.Type == MessageType.Preview || !stream.CanWrite)
				return; // TODO: Exceptions

			using (var write = new StreamWriter(stream))
				write.WriteLine($"[{message.Timestamp.ToString("u", CultureInfo.InvariantCulture)}] {_TypeDict[message.Type]}, {message.Sender?.Name ?? "System"}: {message.BBCodeMessage}");
		}

		public ChatBuffer.MessageData Deserialize(Stream stream)
		{
			if (!stream.CanRead)
				return null; // TODO: Exceptions

			ChatBuffer.MessageData ret = new ChatBuffer.MessageData();

			using (var read = new StreamReader(stream))
			{
				if (read.Read() == '[')
				{
					var data = read.ReadLine();

					var timestamp = data.Substring(0, data.IndexOf(']'));
					data = data.Remove(0, data.IndexOf(']') + 2);
					var type = data.Substring(0, 3);
					data = data.Remove(0, 5);
					var sender = data.Substring(0, data.IndexOf(':'));
					data = data.Remove(0, data.IndexOf(':') + 2);

					ret.Timestamp = DateTime.ParseExact(timestamp, "u", CultureInfo.InvariantCulture);
					ret.Type = _TypeDict.FirstOrDefault(k => k.Value == type).Key;
					ret.Sender = (sender == "System" ? null : Chat.GetOrCreateCharacter(sender));
					ret.RawMessage = new Parser { Validity = NodeValidity.FChat }.ParseMessage(data);
				}
			}

			return ret;
		}
	}
}
