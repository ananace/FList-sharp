using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ConsoleMessenger.Types;
using libflist.FChat;
using libflist.FChat.Commands;
using libflist;
using Newtonsoft.Json;
using ConsoleMessenger.UI;
using ConsoleMessenger.UI.FChat;
using System.Threading;

namespace ConsoleMessenger
{
	public static class StringHelper
	{
		public static string ToString(this IEnumerable<string> en, string joiner)
		{
			return string.Join(joiner, en);
		}

		public static string LongestCommonPrefix(this IEnumerable<string> strs, StringComparison compare = StringComparison.CurrentCulture)
		{
			var stringbuilder = new StringBuilder();
			if (strs == null || !strs.Any())
			{
				return stringbuilder.ToString();
			}

			for (int i = 0; ; i++)
			{
				char ch = char.MaxValue;
				foreach (var str in strs)
				{
					if (i >= str.Length)
					{
						return stringbuilder.ToString();
					}

					if (ch == char.MaxValue)
					{
						ch = str[i];
					}
					else
					{
						if (string.Compare(str[i].ToString(), ch.ToString(), compare) != 0)
						{
							return stringbuilder.ToString();
						}
					}
				}
				stringbuilder.Append(ch);
			}
		}
	}

	public static class OSExtension
	{
		public static bool IsLinux(this OperatingSystem os)
		{
			return os.Platform == PlatformID.MacOSX || os.Platform == PlatformID.Unix || (int)os.Platform == 128;
		}
	}

	public static class ConsoleHelper
	{
        public static event EventHandler OnConsoleResized;

		static Timer _Timer;
        static Size _OldSize;

		public static Size Size { get
			{
				if (Environment.OSVersion.IsLinux())
					return new Size(Console.BufferWidth, Console.BufferHeight);
				return new Size(Console.WindowWidth, Console.WindowHeight);
			} }

		public static void Start()
		{
			_OldSize = Size;
			_Timer = new Timer(HandleTimerCallback, null, 0, 250);
		}
		public static void Stop()
		{
			_Timer.Dispose();
			_Timer = null;
		}

		static void HandleTimerCallback(object state)
		{
			if (OnConsoleResized == null)
				return;

			try
			{
				var newSize = Size;
				if (newSize != _OldSize)
				{
					_OldSize = newSize;
					OnConsoleResized(null, EventArgs.Empty);
				}
			}
			catch (IOException) { }
			catch (ArgumentOutOfRangeException) { }
		}
	}

	public static class Application
	{
		public class StoredTicket
		{
			public AuthTicket Ticket { get; set; }
			public string Account { get; set; }
			public DateTime Timestamp { get; set; }
		}

        public static object DrawLock = new object();

		static List<ChannelBuffer> _ChannelBuffers = new List<ChannelBuffer>();
		static ChannelBuffer _ConsoleBuffer = new ChannelBuffer() { ChatBuf = new ConsoleChatBuffer(), Title = "Console" };
		static InputBox _InputBox = new InputBox();

		static FChatConnection _Chat = new FChatConnection();

		static int _CurBuffer = 0;
        public static ChannelBuffer CurrentChannelBuffer => _ChannelBuffers[_CurBuffer];

		static bool _Running = true;
        static Timer _Redraw = new Timer((_) => { Redraw(); }, null, 1000, 1000);

        public static FChatConnection Connection => _Chat;
		public static StoredTicket Ticket { get; set; }
		public static int CurrentBuffer
		{
			get { return _CurBuffer; }
			set
			{
				if (value != _CurBuffer && value >= 0 && value < _ChannelBuffers.Count)
				{
					_CurBuffer = value;
					CurrentChannelBuffer.Render();
				}
			}
		}
        public static int BufferCount => _ChannelBuffers.Count;

		public static bool Running { get { return _Running; } set { _Running = value; } }
        static string StatusBar { get
            {
                var status = $" {"[".Color(ConsoleColor.DarkCyan)}{DateTime.Now.ToShortTimeString()}{"]".Color(ConsoleColor.DarkCyan)} ";

                if (_Chat.IsIdentified)
                    status += $"{"[".Color(ConsoleColor.DarkCyan)}{Connection.LocalCharacter.Status.ToANSIString()} {Connection.LocalCharacter.ToANSIString()}{"]".Color(ConsoleColor.DarkCyan)} ";

				status += $"{"[".Color(ConsoleColor.DarkCyan)}{_CurBuffer}:{CurrentChannelBuffer.Title ?? "??"}{"]".Color(ConsoleColor.DarkCyan)} ";

				var act = _ChannelBuffers.Where(c => c.Activity).ToList();
				if (act.Any())
				{
					status += $"{"[".Color(ConsoleColor.DarkCyan)}Act: ";
					status += string.Join(",", act.Select(c => {
						string id = _ChannelBuffers.IndexOf(c).ToString();
						return id.Color(c.Hilight ? ConsoleColor.Red : ConsoleColor.White);
					}));
					status += $"{"]".Color(ConsoleColor.DarkCyan)} ";
				}

				return status;
            }
        }

		static void InputLoop()
		{
			Console.InputEncoding = Encoding.UTF8;
			Console.OutputEncoding = Encoding.UTF8;

			while (_Running)
			{
				try
				{
					var key = Console.ReadKey(true);

					if (key.Modifiers.HasFlag(ConsoleModifiers.Alt))
					{
						switch (key.Key)
						{
							case ConsoleKey.D0:
							case ConsoleKey.D1:
							case ConsoleKey.D2:
							case ConsoleKey.D3:
							case ConsoleKey.D4:
							case ConsoleKey.D5:
							case ConsoleKey.D6:
							case ConsoleKey.D7:
							case ConsoleKey.D8:
							case ConsoleKey.D9:
							case ConsoleKey.Q:
							case ConsoleKey.W:
							case ConsoleKey.E:
							case ConsoleKey.R:
							case ConsoleKey.T:
							case ConsoleKey.Y:
							case ConsoleKey.U:
							case ConsoleKey.I:
							case ConsoleKey.O:
							case ConsoleKey.P:
								{ // Switch buffer
									int buf = -1;
									if (key.Key > ConsoleKey.D9)
									{
										IReadOnlyDictionary<char, int> _CharMap = new Dictionary<char, int>{
											{ 'q', 10 },
											{ 'w', 11 },
											{ 'e', 12 },
											{ 'r', 13 },
											{ 't', 14 },
											{ 'y', 15 },
											{ 'u', 16 },
											{ 'i', 17 },
											{ 'o', 18 },
											{ 'p', 19 }
										};
										buf = _CharMap[key.KeyChar];
									}
									else
									{
										buf = (key.Key - ConsoleKey.D1);
										if (buf < 0)
											buf = 9;
									}

									CurrentBuffer = buf;
								}
								break;

							case ConsoleKey.A:
								{ // Go to activity
									var buf = _ChannelBuffers.Where(c => c.Activity || c.Hilight).OrderByDescending(c => (c.Hilight ? 2 : 0) + (c.Activity ? 1 : 0)).FirstOrDefault();

									if (buf != null)
										CurrentBuffer = _ChannelBuffers.IndexOf(buf);
								}
								break;

							case ConsoleKey.H:
							case ConsoleKey.L:
								{ // Next / Prev
									var dir = key.Key == ConsoleKey.H ? -1 : 1;

									var target = CurrentBuffer + dir;
									if (target < 0)
										target = _ChannelBuffers.Count - 1;
									else if (target >= _ChannelBuffers.Count)
										target = 0;
									CurrentBuffer = target;
								}
								break;
						}
					}
					else
					{
						_InputBox.PushInput(key);

						switch (key.Key)
						{
							case ConsoleKey.Tab:
								{
									var inp = _InputBox.Content as string;
									var completion = TabComplete(inp);
									if (completion == null || !completion.Found.Any())
										break;

									var completions = completion.Found;

									var firstSpace = completion.TruePrefix.IndexOf(' ');
									var needsQuotes = firstSpace > -1 && firstSpace < completion.TruePrefix.Length - 1;

									if (inp.EndsWith("\"", StringComparison.OrdinalIgnoreCase))
										inp = inp.Substring(0, inp.Length - 1);
									inp = inp.Substring(0, inp.Length - completion.Prefix.Length);
									if (inp.EndsWith("\"", StringComparison.OrdinalIgnoreCase))
										inp = inp.Substring(0, inp.Length - 1);

									if (completions.Length == 1)
									{
										inp += (needsQuotes ? "\"" : "") + completions.First() + (needsQuotes ? "\"" : "");
									}
									else
									{
										inp += (needsQuotes ? "\"" : "") + completion.TruePrefix;

										// TODO: Show available tab-completions
									}

									_InputBox.Content = inp;
									_InputBox.Cursor = inp.Length;
									_InputBox.Render();
								}
								break;

							case ConsoleKey.Clear: Redraw(true); break;

							default:
								switch (key.KeyChar)
								{
									case '\x04':
										throw new EndOfStreamException();

									case '\f':
										Redraw(true);
										break;
								}
								break;
						}
					}
				}
				catch (EndOfStreamException)
				{
					return;
				}
				catch (TargetInvocationException ex)
				{
					if (ex.InnerException is AggregateException)
					{
						var agg = ex.InnerException as AggregateException;
						foreach (Exception inner in agg.InnerExceptions)
							Debug.WriteLine(inner.Message);
					}
					else
						Debug.WriteLine(ex.InnerException.Message);
				}
				catch (AggregateException ex)
				{
					Debug.WriteLine("{0} exception(s) occured running that command;", ex.InnerExceptions.Count);
					foreach (Exception inner in ex.InnerExceptions)
						Debug.WriteLine("!!  {0}: {1}\n{2}\n", inner.GetType().Name, inner.Message, inner.StackTrace);
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.Message);
				}
			}
		}

		static Command.CompleteResult TabComplete(string input)
		{
			if (!input.StartsWith("/", StringComparison.OrdinalIgnoreCase))
				return null;

			var data = input
				.Substring(1).Split('"')
				.Select((element, index) => index % 2 == 0  // If even index
					? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)  // Split the item
					: new string[] { element })  // Keep the entire item
				.SelectMany(element => element).ToArray();

			IEnumerable<string> avail = null;
			var cmd = data.Any() ? Command.Create(data.First()) : null;
			var used = "";
			if (cmd == null)
			{
				if (!data.Any())
					avail = Command.Available
						.Select(c => c + " ");
				else
				{
					used = data.First();
					avail = Command.Available
						.Where(c => c.Key.StartsWith(data.First(), StringComparison.CurrentCultureIgnoreCase))
						.Select(c => c.Key + " ");
				}
			}
			else if (!input.Contains(' '))
				return new Command.CompleteResult { Prefix = "", TruePrefix = "", Found = new string[] { " " } };
			else
			{
				string[] output;
				used = data.Skip(1).ToString(" ");
				if (cmd.TabComplete(used, out output))
					avail = output;
				else
					return null;
			}

			avail = avail.ToArray();
			var trueprefix = avail.LongestCommonPrefix(StringComparison.CurrentCultureIgnoreCase);
			var prefix = trueprefix;
			if (prefix.Length > used.Length)
				prefix = prefix.Substring(0, used.Length);
			return new Command.CompleteResult { Prefix = prefix, TruePrefix = trueprefix, Found = (string[])avail };
		}

		public static void RunCommand(string command, IEnumerable<string> Args, object source = null)
		{
			var cmd = Command.Create(command);

			if (cmd == null)
				throw new Exception(string.Format("Command not found: {0}", command));

			cmd.Invoke(Args, source);
		}

		public static void TextEntry(string Text, object source = null)
		{
			if (Text.StartsWith("/", StringComparison.OrdinalIgnoreCase))
			{
				// Split, but respect quotation marks
				var data = Text
					.Substring(1).Split('"')
					.Select((element, index) => index % 2 == 0  // If even index
						? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)  // Split the item
						: new string[] { element })  // Keep the entire item
					.SelectMany(element => element)
					.ToArray();

				RunCommand(data.First().ToLower(), data.Skip(1), source);
				return;
			}

			WriteMessage(Text);
		}

		public static void WriteMessage(string Text, Channel inputChan = null, libflist.FChat.Character inputChar = null)
		{
			var Char = inputChar;
			if (Char == null)
				Char = Connection.LocalCharacter;

			var Chan = inputChan;
			ChannelBuffer buffer = null;
			if (Chan == null)
			{
				buffer = _ChannelBuffers[CurrentBuffer];
				if (buffer == _ConsoleBuffer)
					throw new Exception("Can't chat in the console, did you mean to run a command?");

                Chan = buffer.Channel;
			}

			if (buffer == null)
				buffer = _ChannelBuffers.FirstOrDefault(b => b.Channel == Chan);

			if (buffer == null)
				throw new Exception("Message to unknown channel buffer recieved; " + Text);

            buffer.ChatBuf.SendMessage(inputChar, Text);

            buffer.Activity = true;
            if (Text.ToLower().Contains(Connection.LocalCharacter.Name.ToLower()))
                buffer.Hilight = true;

            if (CurrentChannelBuffer == buffer)
                CurrentChannelBuffer.Render();
			// Redraw();
		}

		public static void Redraw(bool full = false)
		{
            lock (DrawLock)
            {
                if (full)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.SetCursorPosition(0, 0);

                    Console.Clear();

                    _ChannelBuffers[_CurBuffer].Render();

                    _InputBox.Render();
                }

				var size = ConsoleHelper.Size;
                Graphics.DrawLine(new Point(0, size.Height - 2), new Point(size.Width - 1, size.Height - 2), ' ', ConsoleColor.DarkBlue);
                Graphics.WriteANSIString(StatusBar, new Point(0, size.Height - 2), ConsoleColor.DarkBlue, ConsoleColor.Gray);

                _InputBox.Focus();
            }
		}

		public static void Run()
		{
			Console.Title = string.Format("FChat Messenger v{0}", Assembly.GetExecutingAssembly().GetName().Version);

            Debug.Listeners.Clear();
            Debug.Listeners.Add((_ConsoleBuffer.ChatBuf as ConsoleChatBuffer).TraceListener);
            _ChannelBuffers.Add(_ConsoleBuffer);
            _InputBox.OnTextEntered += (s, e) => { TextEntry(e); };

			_Chat.Endpoint = FChatConnection.TestingServerEndpoint;
			//_Chat.Endpoint = FChatConnection.LiveServerEndpoint;

			_Chat.OnErrorMessage += (_, e) => 
				_ConsoleBuffer.ChatBuf.PushMessage(null, (e.Command as Server_ERR_ChatError).Error);

			_Chat.OnChannelJoin += (_, e) =>
			{
				ChannelBuffer chatBuf = _ChannelBuffers.FirstOrDefault(c => c.Channel == e.Channel);
				if (chatBuf == null)
				{
					chatBuf = new ChannelBuffer { Channel = e.Channel, Title = e.Channel.Title };
					_ChannelBuffers.Add(chatBuf);

				}
				CurrentBuffer = _ChannelBuffers.IndexOf(chatBuf);
			};
			_Chat.OnChannelChatMessage += (_, e) => WriteMessage(e.Message, e.Channel, e.Character);
			_Chat.OnChannelRollMessage += (_, e) =>
			{
				var roll = e.Command as Server_RLL_ChannelRollMessage;
				WriteMessage(roll.Message, e.Channel, e.Character);
			};
			_Chat.OnCharacterChatMessage += (_, e) =>
			{
				WriteMessage(e.Message, null, e.Character);
			};
			_Chat.OnChannelLeave += (_, e) =>
			{
				int i = 0;
				foreach (var c in _ChannelBuffers)
				{
					if (c.Channel == e.Channel)
					{
						if (_CurBuffer == i)
							_CurBuffer--;

						_ChannelBuffers.Remove(c);
						break;
					}
					++i;
				}
			};

			Redraw(true);
			InputLoop();

			if (_Chat != null)
				_Chat.Dispose();
			_Chat = null;
		}


		public static void Main(string[] args)
		{
			Ticket = LoadTicket();

			ConsoleHelper.Start();
			Run();
			ConsoleHelper.Stop();

			SaveTicket(Ticket);
		}

		public static StoredTicket LoadTicket()
		{
			if (File.Exists("ticket.json"))
			{
				var data = File.ReadAllText("ticket.json");
				var stored = JsonConvert.DeserializeObject<StoredTicket>(data);

				if (DateTime.Now - stored.Timestamp > TimeSpan.FromHours(24))
				{
					Debug.WriteLine("Old ticket has timed out, please reconnect with /connect <user> <password>");
					return null;
				}

				Debug.WriteLine("Old ticket found, connect with /connect");
				return stored;
			}
			return null;
		}
		public static void SaveTicket(StoredTicket ticket)
		{
			if (ticket != null)
			{
				var stored = JsonConvert.SerializeObject(ticket);
				File.WriteAllText("ticket.json", stored);
			}
		}
	}
}