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

        public bool Activity { get; set; }
        public bool Hilight { get; set; }

        string TitleBar
        {
            get
            {
                return Title;
            }
        }

        public void Render()
        {
            lock (Application.DrawLock)
            {
                Activity = false;
                Hilight = false;

                Graphics.DrawLine(new Point(0, 0), new Point(ConsoleHelper.Size.Width - 1, 0), ' ', ConsoleColor.DarkBlue);
                Graphics.WriteANSIString(TitleBar, new Point(0, 0), ConsoleColor.DarkBlue, ConsoleColor.White);
                Graphics.DrawFilledBox(new Point(0, 1), new Point(ConsoleHelper.Size.Width - 1, ConsoleHelper.Size.Height - 3), ' ');

                int height = ConsoleHelper.Size.Height - 4;
                int totalHeight = 0;

                // TODO Scrolling

                using (var c = new CursorChanger(new Point(0, 1)))
                {
                    foreach (var msg in ChatBuf.Messages.Reverse().TakeWhile(p =>
                        {
                            if (totalHeight >= height)
                                return false;

                            int len = p.Message.ANSILength()
                                + (p.Timestamp.Date == DateTime.Now ? 7 : 12)
                                + 2;
                            if (p.Sender != null)
                                len += p.Sender.Name.Length;

                            if (totalHeight + len / ConsoleHelper.Size.Width > height)
                            {
                                totalHeight = height;
                                return false;
                            }

                            totalHeight += len / ConsoleHelper.Size.Width;
                            return true;
                        }).Reverse())
                    {
                        if (msg.Timestamp.Date == DateTime.Now.Date)
                            Graphics.WriteANSIString($"[{msg.Timestamp.ToShortTimeString()}] ".Color(ConsoleColor.Gray));
                        else
                            Graphics.WriteANSIString($"[{msg.Timestamp.ToShortDateString()}] ".Color(ConsoleColor.Gray));

                        bool action = msg.Message.StartsWith("/me ", StringComparison.CurrentCultureIgnoreCase);
                        if (action)
                            Console.Write("* ");

						if (msg.Sender != null)
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
							Console.CursorLeft++;
							Graphics.WriteANSIString(msg.Message.Substring(4), foreground: ConsoleColor.White);
						}
						else
							Graphics.WriteANSIString(": " + msg.Message, foreground: ConsoleColor.Gray);

                        Console.CursorLeft = 0;
                        Console.CursorTop++;
                    }
                }
            }
        }
    }
}
