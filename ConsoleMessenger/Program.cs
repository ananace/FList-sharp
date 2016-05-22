using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ConsoleMessenger.Types;
using ConsoleMessenger.UI.Panels;
using libflist.FChat;
using libflist.JSON.Responses;
using Newtonsoft.Json;
using ConsoleMessenger.UI;
using ConsoleMessenger.UI.FChat;

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

	public static class ConsoleHelper
	{
		static System.Threading.Timer _Timer;
		static Point _OldSize;

		public static event EventHandler<Point> OnConsoleResized;

		public static void Start()
		{
			_OldSize = new Point(Console.WindowWidth, Console.WindowHeight);
			_Timer = new System.Threading.Timer(HandleTimerCallback, null, 0, 250);
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
				var newSize = new Point(Console.WindowWidth, Console.WindowHeight);

				if (newSize != _OldSize)
				{
					_OldSize = newSize;
					OnConsoleResized(null, newSize);
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
			public TicketResponse Ticket { get; set; }
			public string Account { get; set; }
			public DateTime Timestamp { get; set; }
		}

		static Panel _Root;
		static VerticalPanel _RootStack;
		static TitledPanel _MainPanel;
		static TitledPanel _StatusPanel;
		static HorizontalPanel _InputPanel;
		static ContentControl _ChannelInfo;
		static UI.FChat.StatusBar _StatusBar;
		static List<UI.FChat.ChannelBuffer> _ChannelBuffers = new List<UI.FChat.ChannelBuffer>();
		static UI.FChat.ChannelBuffer _ConsoleBuffer;
		static InputControl _InputBox;
		static FChatConnection _Chat = new FChatConnection();

		static int _CurBuffer = 0;

		static bool _Running = true;
		static System.Threading.Timer _Redraw;

		public static FChatConnection Connection { get { return _Chat; } }
		public static StoredTicket Ticket { get; set; }
		public static int CurrentBuffer
		{
			get { return _CurBuffer; }
			set
			{
				_CurBuffer = value;

				var buf = _ChannelBuffers[value];
				_MainPanel.Child = buf;
				_ChannelInfo.Content = string.Format("[{0}]", (buf.Tag is Channel) ? (buf.Tag as Channel).Title : "(Console)");
			}
		}
		public static int BufferCount { get { return _ChannelBuffers.Count; } }

		public static bool Running { get { return _Running; } set { _Running = value; } }

		static void BuildUI()
		{
			_Root = new Panel();
			_RootStack = new VerticalPanel()
			{
				Sizing = SizingHint.FillAvailable
			};
			_MainPanel = new TitledPanel()
			{
				TitleColor = ConsoleColor.Blue,
				ResizeChildren = true
			};
			_StatusPanel = new TitledPanel();
			_InputPanel = new HorizontalPanel()
			{
				Background = ConsoleColor.Black,
				Padding = new Rect(0, 0, 1, 0)
			};
			_StatusBar = new UI.FChat.StatusBar()
			{
				Chat = _Chat,
				Margin = new Rect(1, 0, 1, 0)
			};
			_ConsoleBuffer = new UI.FChat.ChannelBuffer()
			{
				Background = ConsoleColor.Black,
				Foreground = ConsoleColor.Gray
			};
			_ChannelBuffers.Add(_ConsoleBuffer);
			Debug.Listeners.Add(_ConsoleBuffer.TraceListener);

			_MainPanel.TitleColor = ConsoleColor.Blue;
			_StatusPanel.TitleColor = ConsoleColor.Blue;
			_StatusPanel.Size = new Point(0, 2);

			_ChannelInfo = new ContentControl()
			{
				Content = "[(Console)]",
				Foreground = ConsoleColor.Gray,
				Margin = new Rect(0, 0, 1, 0),
				Sizing = SizingHint.ShrinkToFit
			};
			_InputBox = new InputControl()
			{
				Sizing = SizingHint.ShrinkToFit
			};
			_InputBox.OnTextEntered += (box, text) => TextEntry(text, box);

			_InputPanel.Children.Add(_ChannelInfo);
			_InputPanel.Children.Add(_InputBox);

			_StatusPanel.TitleControl = _StatusBar;
			_StatusPanel.Child = _InputPanel;

			_MainPanel.Child = _ConsoleBuffer;

			_RootStack.Children.Add(_MainPanel);
			_RootStack.Children.Add(_StatusPanel);

			_Root.Child = _RootStack;

			_Redraw = new System.Threading.Timer((_) => Redraw(), null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

			ConsoleHelper.OnConsoleResized += (_, __) =>
			{
				Redraw(true);
			};
			_Root.OnVisualInvalidated += (_, __) =>
			{
				_Redraw.Change(10, System.Threading.Timeout.Infinite);
			};
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
							}
							break;

						case ConsoleKey.Clear: Redraw(true); break;

						default:
							switch (key.KeyChar)
							{
								case '1':
								case '2':
								case '3':
								case '4':
								case '5':
								case '6':
								case '7':
								case '8':
								case '9':
									{
										int buffer = key.KeyChar - '1';

										if (buffer <= BufferCount && key.Modifiers.HasFlag(ConsoleModifiers.Alt))
											CurrentBuffer = buffer;
									}
									break;

								case '\x04':
									throw new EndOfStreamException();

								case '\f':
									Redraw(true);
									break;
							}
							break;
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
					.SelectMany(element => element);

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

			var trueprefix = avail.LongestCommonPrefix(StringComparison.CurrentCultureIgnoreCase);
			var prefix = trueprefix;
			if (prefix.Length > used.Length)
				prefix = prefix.Substring(0, used.Length);
			return new Command.CompleteResult { Prefix = prefix, TruePrefix = trueprefix, Found = avail.ToArray() };
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
					.SelectMany(element => element);

				RunCommand(data.First().ToLower(), data.Skip(1), source);
				return;
			}

			WriteMessage(Text);
		}

		public static void WriteMessage(string Text, Channel inputChan = null, Character inputChar = null)
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

				Chan = (buffer.Tag as Channel);
			}

			if (buffer == null)
				buffer = _ChannelBuffers.FirstOrDefault(b => b.Tag == Chan);

			if (buffer == null)
				throw new Exception("Message to unknown channel buffer recieved; " + Text);

			if (inputChar == null)
				Chan.SendMessage(Text);

			if (Text.StartsWith("/me", StringComparison.OrdinalIgnoreCase))
				buffer.PushMessage(string.Format("{2} {0} {1}",
					inputChar == null ?
						Char.Name.Color(Char.GenderColor).BackgroundColor(ConsoleColor.DarkGray) :
						Char.Name.Color(Char.GenderColor),
					Text.Length > 3 ? Text.Substring(4).Color(ConsoleColor.White) : "",
					Char.StatusChar.ToString().Color(Char.StatusColor)
					));
			else
				buffer.PushMessage(string.Format("{2} {0}: {1}",
					inputChar == null ?
						Char.Name.Color(Char.GenderColor).BackgroundColor(ConsoleColor.DarkGray) :
						Char.Name.Color(Char.GenderColor),
					Text,
					Char.StatusChar.ToString().Color(Char.StatusColor)
					));
		}

		public static void Redraw(bool full = false)
		{
			if (_Root == null)
				return;

			if (full)
			{
				Console.BackgroundColor = ConsoleColor.Black;
				Console.ForegroundColor = ConsoleColor.White;
				Console.SetCursorPosition(0, 0);

				Console.Clear();

				var size = Graphics.AvailableSize;

				_Root.Size = size;
				_MainPanel.Size = size - new Size(0, 2);
				_StatusPanel.Size = new Size(size.Width, 2);
				_InputPanel.Size = new Size(size.Width, 1);

				_Root.InvalidateLayout();
				_Root.InvalidateVisual();
			}

			_Root.Draw();

			_InputBox.Focus();
		}

		public static void Run()
		{
			BuildUI();

			Console.Title = string.Format("FChat Messenger v{0}", Assembly.GetExecutingAssembly().GetName().Version);

			//_Chat.Endpoint = FChatConnection.TestingServerEndpoint;
			_Chat.Endpoint = FChatConnection.LiveServerEndpoint;

			_Chat.OnIdentified += (_, __) => _StatusBar.InvalidateVisual();

			_Chat.OnErrorMessage += (_, e) => 
				_ConsoleBuffer.PushMessage((e.Command as libflist.FChat.Commands.Server.ChatError).Error);

			_Chat.OnChannelJoin += (_, e) =>
			{
				var chatBuf = new ChannelBuffer
				{
					Tag = e.Channel,
					Background = ConsoleColor.Black,
					Foreground = ConsoleColor.Gray
				};

				_ChannelBuffers.Add(chatBuf);
				CurrentBuffer = _ChannelBuffers.Count - 1;
			};
			_Chat.OnChannelChatMessage += (_, e) => WriteMessage(e.Message, e.Channel, e.Character);
			_Chat.OnChannelRollMessage += (_, e) => {
				var roll = e.Command as libflist.FChat.Commands.Server.Channel.SendRollReply;
				WriteMessage(roll.Message, e.Channel, e.Character);
			};
			_Chat.OnChannelLeave += (_, e) =>
			{
				int i = 0;
				foreach (var c in _ChannelBuffers)
				{
					if (c.Tag == e.Channel)
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
			if (_Root != null)
				_Root.Dispose();
			_Root = null;
		}


		public static void Main(string[] args)
		{
			Application.Ticket = LoadTicket();

			ConsoleHelper.Start();
			Application.Run();
			ConsoleHelper.Stop();

			SaveTicket(Application.Ticket);
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