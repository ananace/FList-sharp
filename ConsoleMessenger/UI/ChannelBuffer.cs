using ConsoleMessenger.Settings;
using ConsoleMessenger.Types;
using libflist.FChat;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleMessenger.UI.FChat
{
	public class ChannelBuffer
    {
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
						Render();
				};
			}
		}
		public Channel Channel => (_ChatBuf as ChannelChatBuffer)?.Channel;
		public Character Character => (_ChatBuf as CharacterChatBuffer)?.Character;

		public string Title { get; set; }

        public bool SystemActivity { get; set; }
		public bool Activity { get; set; }
        bool _Hilight;
		public bool Hilight
        {
            get { return _Hilight; }
            set
            {
                if (value && !_Hilight && BellOnHighlight)
                    Console.Beep();
                _Hilight = value;
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

                if ((msg[MessageDisplay].EndsWith("\n", StringComparison.Ordinal))
                    mheight--;

                msg.Lines = mheight;
            }
        }

        public void Render()
        {
            lock (Application.DrawLock)
            {
                RenderMessages();

                SystemActivity = false;
                Activity = false;
                Hilight = false;

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
                        {
                            if (Channel != null)
                            {
                                if (msg.Sender.IsChatOp)
                                    Graphics.WriteANSIString("▼".Color(ConsoleColor.Yellow));

                                if (Channel.Official)
                                {
                                    if (Channel.Owner == msg.Sender || msg.Sender.IsOPInChannel(Channel))
                                        Graphics.WriteANSIString("▼".Color(ConsoleColor.Red));
                                }
                                else
                                {
                                    if (Channel.Owner == msg.Sender)
                                        Graphics.WriteANSIString("►".Color(ConsoleColor.Cyan));
                                    else if (msg.Sender.IsOPInChannel(Channel))
                                        Graphics.WriteANSIString("►".Color(ConsoleColor.Red));
                                }
                            }
                            Graphics.WriteANSIString(msg.Sender.ToANSIString());
                        }
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
