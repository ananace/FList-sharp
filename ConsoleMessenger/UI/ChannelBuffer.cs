using ConsoleMessenger.Settings;
using ConsoleMessenger.Types;
using libflist.FChat;
using libflist.Message;
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

            SystemActivity = 1 << 0,
            ChatActivity = 1 << 1,
            Highlight = 1 << 2
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

        public static int SCROLL_BOTTOM = int.MinValue;
        public int _ScrollValue = SCROLL_BOTTOM;
        public int Scroll
        {
            get
            {
                if (_ScrollValue == SCROLL_BOTTOM)
                    return Math.Max(0, RenderedMessages.Count() - Size.Height);

                return _ScrollValue;
            }
            set
            {
                if (value == SCROLL_BOTTOM ||
                    (value > 0 && value >= (RenderedMessages.Count() - Size.Height)))
                    _ScrollValue = SCROLL_BOTTOM;
                else
                {
                    if (value < 0)
                        _ScrollValue = 0;
                    else
                        _ScrollValue = value;
                }
            }
        }

        public Size Size
        {
            get
            {
                return ConsoleHelper.Size - new Size(0, 4);
            }
        }

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

        IEnumerable<KeyValuePair<ChatBuffer.MessageData, ANSIString>> RenderedMessages
        {
            get
            {
                foreach (var msg in Messages)
                {
                    var text = RenderMessage(msg);
                    var splits = new AutoSplitString(text);

                    foreach (var line in splits.SplitString)
                        yield return new KeyValuePair<ChatBuffer.MessageData, ANSIString>(msg, line);
                }
            }
        }

		// TODO: Pre-render messages into row-lists, split on \n and width.
        ANSIString RenderMessage(ChatBuffer.MessageData msg)
        {
            if (msg.RenderedMessage == null)
            {
                var build = new ANSIString();

                if (msg.Timestamp.Date == DateTime.Now.Date)
                    build.Append($"[{msg.Timestamp.ToString("HH:mm")}]".Color(ConsoleColor.Gray));
                else
                    build.Append($"[{msg.Timestamp.ToString("yyyy-MM-dd")}]".Color(ConsoleColor.Gray));

                ANSIString message = new ANSIRenderer().Render(msg.RawMessage);
                bool action = message.PlainString.StartsWith("/me", StringComparison.CurrentCultureIgnoreCase);
                build.Append(action ? "*" : " ");

                if (msg.Sender?.Name != null)
                    build.Append(msg.Sender.ToANSIString(Channel, true));
                else
                    build.Append("System".Color(ConsoleColor.DarkGray));

                if (action)
                {
                    if (message.PlainString.StartsWith("/me's", StringComparison.CurrentCultureIgnoreCase))
                        build.Append(message.Substring(3));
                    else
                        build.Append(new ANSIString(" ") + message.Substring(4));
                }
                else
                    build.Append(new ANSIString(": ") + message);

                msg.RenderedMessage = build;
            }

            return msg.RenderedMessage;
        }
		
		// TODO: Speed up rendering if possible
        public void Render()
        {
            lock (Application.DrawLock)
            {
				NeedsRender = false;

                _Activity = ActivityFlags.None;

                Graphics.DrawLine(new Point(0, 0), new Point(ConsoleHelper.Size.Width - 1, 0), ' ', ConsoleColor.DarkBlue);
                Graphics.WriteANSIString(new ANSIString(TitleBar), new Point(0, 0), ConsoleColor.DarkBlue, ConsoleColor.White);
                Graphics.DrawFilledBox(new Point(0, 1), new Point(ConsoleHelper.Size.Width - 1, ConsoleHelper.Size.Height - 2), ' ');

                using (var c = new CursorChanger(new Point(0, 1)))
                {
                    lock (_ChatBuf)
                    {
                        var rendered = RenderedMessages.ToArray();
                        var shown = Math.Min(rendered.Length, Size.Height);

                        var scroll = Scroll;

                        for (int i = 0; i < shown; ++i)
                        {
                            var line = rendered[i + scroll];

                            Graphics.WriteANSIString(line.Value);

                            Console.CursorLeft = 0;
                            Console.CursorTop++;
                        }
                    }
                }
            }
        }
    }
}
