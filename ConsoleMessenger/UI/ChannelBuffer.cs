using ConsoleMessenger.Settings;
using ConsoleMessenger.Types;
using libflist.FChat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ConsoleMessenger.Logging;
using System.Text;

namespace ConsoleMessenger.UI.FChat
{
	public class ChannelBuffer
    {
        [Flags]
        public enum ActivityFlags
        {
            None = 0,

            SystemActivity = 1<<0,
            ChatActivity = 1<<1,
            Highlight = 1<<2
        }

		ChatBuffer _ChatBuf;
		public ChatBuffer ChatBuf
		{
			get { return _ChatBuf; }
			set
			{
				_ChatBuf = value;
				_ChatBuf.OnMessage += (s, e) =>
				{
					if (Application.CurrentChannelBuffer == this)
						NeedsRender = true;

					if (LogMessages && e.Type != MessageType.Preview)
					{
						using (var stream = new FileStream($"{Title.ToLower()}.log", System.IO.FileMode.Append))
							new MessageSerializer(Channel?.Connection ?? Character?.Connection).Serialize(stream, e);
					}
				};
			}
		}
		public Channel Channel => (_ChatBuf as ChannelChatBuffer)?.Channel;
		public Character Character => (_ChatBuf as CharacterChatBuffer)?.Character;

		public bool NeedsRender { get; set; }

        string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title == null && File.Exists($"{value.ToLower()}.log"))
                {
                    var ser = new MessageSerializer(Channel?.Connection ?? Character?.Connection);
                    var lines = File.ReadAllLines($"{value.ToLower()}.log").Reverse().Take(100).Reverse().ToArray();

                    var msgs = new List<ChatBuffer.MessageData>();
                    foreach (var line in lines)
                        using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                        {
                            msgs.Add(ser.Deserialize(stream));
                        }

                    _ChatBuf.Clear();
                    foreach (var msg in msgs)
                        _ChatBuf.PushMessage(msg);
                }

                _Title = value;
            }
        }

        ActivityFlags _Activity = ActivityFlags.None;
        public bool SystemActivity
        {
            get { return _Activity.HasFlag(ActivityFlags.SystemActivity); }
            set
            {
                if (value)
                    _Activity = _Activity | ActivityFlags.SystemActivity;
                else
                    _Activity = _Activity & ~ActivityFlags.SystemActivity;
            }
        }
		public bool ChatActivity
        {
            get { return _Activity.HasFlag(ActivityFlags.ChatActivity); }
            set
            {
                if (value)
                    _Activity = _Activity | ActivityFlags.ChatActivity;
                else
                    _Activity = _Activity & ~ActivityFlags.ChatActivity;
            }
        }
        public bool Highlight
        {
            get { return _Activity.HasFlag(ActivityFlags.Highlight); }
            set
            {
                if (value)
                    _Activity = _Activity | ActivityFlags.Highlight;
                else
                    _Activity = _Activity & ~ActivityFlags.Highlight;
            }
        }

		public int Scroll { get; set; } = -1;

		[Setting("buffer.show_ads", DefaultValue = true, Description = "Should the buffer display ads?")]
		public bool ShowADs { get; set; } = true;
		[Setting("buffer.show_messages", DefaultValue = true, Description = "Should the buffer display messages?")]
		public bool ShowMessages { get; set; } = true;

		// TODO: ANSI generation for BBCodes
		[Setting("buffer.messagetype", DefaultValue = true, Description = "How to display messages in the buffer")]
        public MessageDisplayType MessageDisplay { get; set; } = MessageDisplayType.Plain;

        [Setting("buffer.sys_timeout", DefaultValue = null, Description = "Provide a timeout value in seconds for system messages to be shown in the buffer")]
        public double? TimeoutSys { get; set; } = null;
        [Setting("buffer.preview_timeout", DefaultValue = null, Description = "Provide a timeout value in seconds for preview messages to be shown in the buffer")]
        public double? TimeoutPre { get; set; } = null;

        [Setting("buffer.bell_on_higlight", DefaultValue = false, Description = "Play a console bell when someone mentions your name in the buffer")]
        public bool BellOnHighlight { get; set; } = false;
        [Setting("buffer.log_messages", DefaultValue = false, Description = "Log messages to a <channel>.log file")]
        public bool LogMessages { get; set; } = false;

		[Setting("buffer.max_messages", DefaultValue = 100, Description = "The number of messages to store in scrollback.")]
		public int MaxMessages
		{
			get { return _ChatBuf.MaxMessages; }
			set { _ChatBuf.MaxMessages = value; }
		}

        string TitleBar
        {
            get
            {
                return Title;
            }
        }

		IEnumerable<ChatBuffer.MessageData> Messages
		{
			get
            {
                IEnumerable<ChatBuffer.MessageData> en = _ChatBuf.Messages;
                if (TimeoutSys.HasValue)
                    en = en.Where(m => m.Sender != null || (DateTime.Now - m.Timestamp) <= TimeSpan.FromSeconds(TimeoutSys.Value));
                if (TimeoutPre.HasValue)
                    en = en.Where(m => m.Type != MessageType.Preview || (DateTime.Now - m.Timestamp) <= TimeSpan.FromSeconds(TimeoutPre.Value));
				if (!ShowADs)
					en = en.Where(m => m.Type != MessageType.LFRP);
				if (!ShowMessages)
					en = en.Where(m => m.Type != MessageType.Chat);

				return en;
			}
		}

		// TODO: Pre-render messages into row-lists, split on \n and width.
        void RenderMessages()
        {
			lock(_ChatBuf)
            foreach (var msg in Messages)
            {
                int len = (msg.Timestamp.Date == DateTime.Now ? 7 : 12) + 2;
                if (msg.Sender?.Name != null)
                    len += msg.Sender.Name.Length;
                else
                    len += 6;

                int mheight = 1;
                foreach (var ch in msg[MessageDisplay])
                    if (++len > ConsoleHelper.Size.Width || ch == '\n')
                    {
                        mheight++;
                        len = 0;
                    }

                if ((msg[MessageDisplay].EndsWith("\n", StringComparison.Ordinal)))
                    mheight--;

                msg.Lines = mheight;
            }
        }
		
		// TODO: Speed up rendering if possible
        public void Render()
        {
            lock (Application.DrawLock)
            {
				NeedsRender = false;
                RenderMessages();

                _Activity = ActivityFlags.None;

                Graphics.DrawLine(new Point(0, 0), new Point(ConsoleHelper.Size.Width - 1, 0), ' ', ConsoleColor.DarkBlue);
                Graphics.WriteANSIString(TitleBar, new Point(0, 0), ConsoleColor.DarkBlue, ConsoleColor.White);
                Graphics.DrawFilledBox(new Point(0, 1), new Point(ConsoleHelper.Size.Width - 1, ConsoleHelper.Size.Height - 2), ' ');

                int height = ConsoleHelper.Size.Height - 4;
                int totalHeight = 0;

                // TODO Scrolling

                using (var c = new CursorChanger(new Point(0, 1)))
                {
					lock(_ChatBuf)
                    foreach (var msg in Messages
                        .Reverse()
                        .TakeWhile(p => (totalHeight += p.Lines) < height)
                        .Reverse()
                        .Skip(totalHeight > height ? 1 : 0))
                    {
                        if (msg.Timestamp.Date == DateTime.Now.Date)
                            Graphics.WriteANSIString($"[{msg.Timestamp.ToString("HH:mm")}] ".Color(ConsoleColor.Gray));
                        else
                            Graphics.WriteANSIString($"[{msg.Timestamp.ToString("yyyy-MM-dd")}] ".Color(ConsoleColor.Gray));

                        string message = msg[MessageDisplay];
                        bool action = message.StartsWith("/me", StringComparison.CurrentCultureIgnoreCase);
                        if (action)
                            Console.Write("* ");

                        if (msg.Sender?.Name != null)
                            Graphics.WriteANSIString(msg.Sender.ToANSIString(Channel, true));
                        else
                            Graphics.WriteANSIString("System".Color(ConsoleColor.DarkGray));

                        if (action)
                        {
                            if (msg.PlainMessage.StartsWith("/me's", StringComparison.CurrentCultureIgnoreCase))
                            {
                                Graphics.WriteANSIString(message.Substring(3), foreground: ConsoleColor.White);
                            }
                            else
                            {
                                Console.CursorLeft++;
                                Graphics.WriteANSIString(message.Substring(4), foreground: ConsoleColor.White);
                            }
                        }
                        else
                            Graphics.WriteANSIString(": " + message, foreground: ConsoleColor.Gray);

                        Console.CursorLeft = 0;
                        Console.CursorTop++;
                    }
                }
            }
        }
    }
}
