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

        public int Scroll { get; set; } = -1;

        string TitleBar
        {
            get
            {
                return Title;
            }
        }

        void RenderMessages()
        {
            foreach (var msg in _ChatBuf.Messages)
            {
                int len = (msg.Timestamp.Date == DateTime.Now ? 7 : 12) + 2;
                if (msg.Sender?.Name != null)
                    len += msg.Sender.Name.Length;
                else
                    len += 6;

                int mheight = 1;
                foreach (var ch in msg.Message.ToPlainString())
                    if (++len > ConsoleHelper.Size.Width || ch == '\n')
                    {
                        mheight++;
                        len = 0;
                    }

                if (msg.Message.EndsWith("\n", StringComparison.Ordinal))
                    mheight--;

                msg.Lines = mheight;
            }
        }

        public void Render()
        {
            lock (Application.DrawLock)
            {
                RenderMessages();

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
                    foreach (var msg in ChatBuf.Messages
                        .Reverse()
                        .TakeWhile(p => (totalHeight += p.Lines) < height)
                        .Reverse()
                        .Skip(totalHeight > height ? 1 : 0))
                    {
                        if (msg.Timestamp.Date == DateTime.Now.Date)
                            Graphics.WriteANSIString($"[{msg.Timestamp.ToString("HH:mm")}] ".Color(ConsoleColor.Gray));
                        else
                            Graphics.WriteANSIString($"[{msg.Timestamp.ToString("yyyy-MM-dd")}] ".Color(ConsoleColor.Gray));

                        bool action = msg.Message.StartsWith("/me", StringComparison.CurrentCultureIgnoreCase);
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
                            if (msg.Message.StartsWith("/me's", StringComparison.CurrentCultureIgnoreCase))
                            {
                                Graphics.WriteANSIString(msg.Message.Substring(3), foreground: ConsoleColor.White);
                            }
                            else
                            {
                                Console.CursorLeft++;
                                Graphics.WriteANSIString(msg.Message.Substring(4), foreground: ConsoleColor.White);
                            }
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
