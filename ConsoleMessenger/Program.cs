using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ConsoleMessenger.Types;
using ConsoleMessenger.UI.Panels;
using libflist;
using libflist.JSON.Responses;
using Newtonsoft.Json;
using ConsoleMessenger.UI;

namespace ConsoleMessenger
{
	public static class StringHelper
	{
		public static string ToString(this IEnumerable<string> en, string joiner)
		{
			return string.Join(joiner, en);
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

		static VerticalPanel _Root;
		static TitledPanel _MainPanel;
		static TitledPanel _StatusPanel;
		static HorizontalPanel _InputPanel;
		static ContentControl _ChannelInfo;
		static UI.FChat.StatusBar _StatusBar;
		static List<UI.FChat.ChannelBuffer> _ChannelBuffers = new List<UI.FChat.ChannelBuffer>();
		static UI.FChat.ChannelBuffer _ConsoleBuffer;
		static InputControl _InputBox;
		static FChat _Chat = new FChat();

		static int _CurBuffer = 0;

		static bool _Running = true;
		static System.Threading.Timer _Redraw;

		public static FChat Connection { get { return _Chat; } }
		public static StoredTicket Ticket { get; set; }
		public static int CurrentBuffer
		{
			get { return _CurBuffer; }
			set
			{
				_CurBuffer = value;

				_MainPanel.Child = _ChannelBuffers[value];
			}
		}
		public static int BufferCount { get { return _ChannelBuffers.Count; } }

		public static bool Running { get { return _Running; } set { _Running = value; } }

		static void BuildUI()
		{
			_Root = new VerticalPanel();
			_MainPanel = new TitledPanel()
			{
				TitleColor = ConsoleColor.Blue,
				ResizeChildren = true
			};
			_StatusPanel = new TitledPanel();
			_InputPanel = new HorizontalPanel();
			_StatusBar = new UI.FChat.StatusBar()
			{
				Margin = new Rect(1, 0, 1, 0)
			};
			_ConsoleBuffer = new UI.FChat.ChannelBuffer()
			{
				
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
                Margin = new Rect(0, 0, 1, 0)
			};
			_InputBox = new InputControl()
			{
			};
			_InputBox.OnTextEntered += (_, text) => TextEntry(text);

			_InputPanel.Children.Add(_ChannelInfo);
			_InputPanel.Children.Add(_InputBox);

			_StatusPanel.TitleControl = _StatusBar;
			_StatusPanel.Child = _InputPanel;

			_MainPanel.Child = _ConsoleBuffer;

			_Root.Children.Add(_MainPanel);
			_Root.Children.Add(_StatusPanel);

			_Redraw = new System.Threading.Timer((_) => Redraw(), null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

			ConsoleHelper.OnConsoleResized += (_, __) => {
				Redraw(true);
			};
			_Root.OnVisualInvalidated += (_, __) => {
				_Redraw.Change(10, System.Threading.Timeout.Infinite);
			};
		}

		static void Connect()
		{
			if (Ticket == null)
				Console.WriteLine("No usable ticket found, please log in below;");

			while (Ticket == null)
			{
				Console.Write("Username> ");
				var user = Console.ReadLine();
				Console.Write("Password> ");
				var pass = Console.ReadLine();

				var rand = new Random();
				Console.CursorTop -= 1;
				Console.CursorLeft = 0;
				Console.WriteLine("Password> {0}", new string('*', pass.Length + rand.Next(0, pass.Length)));

				_Chat.Connect(user, pass);

				if (_Chat.Ticket.Successful)
					Ticket = new StoredTicket {
					Ticket = _Chat.Ticket,
					Account = user,
					Timestamp = DateTime.Now
				};
				else
					Console.WriteLine("Failed to retrieve a working ticket; {0}", _Chat.Ticket.ErrorData ?? _Chat.Ticket.Error);
			}

			if (!_Chat.Connection.Connected)
			{
				_Chat.Ticket = Ticket.Ticket;
				_Chat.TicketTimestamp = Ticket.Timestamp;
				_Chat.Connect(Ticket.Account, null, true);
			}
		}
		static void Login()
		{
			Console.WriteLine("Choose your character;\n  Available characters: {0}\nCharacter> ", string.Join(", ", _Chat.User.Characters));
			var character = Console.ReadLine();

			_Chat.Login(character);
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
                            var found = TabComplete(inp);
							if (found == null)
								break;

							if (found.Length == 1)
							{
								if (inp.Contains(' '))
									inp = inp.Remove(inp.LastIndexOf(' ') + 1) + found.First();
								else
									inp = (inp.Length > 1 ? inp.Remove(1) : inp) + found.First();
							}

							_InputBox.Content = inp;
						} break;
							
					case ConsoleKey.Clear: Redraw(true); break;

					default:
						switch(key.KeyChar)
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
							} break;

						case '\x04':
							throw new EndOfStreamException();

						case '\f':
							Redraw(true);
							break;
						} break;
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
				catch(AggregateException ex)
				{
					Debug.WriteLine("{0} exception(s) occured running that command;", ex.InnerExceptions.Count);
					foreach (Exception inner in ex.InnerExceptions)
						Debug.WriteLine("!!  {0}: {1}\n{2}\n", inner.GetType().Name, inner.Message, inner.StackTrace);
				}
				catch(Exception ex)
				{
					Debug.WriteLine(ex.Message);
				}
			}
		}

		static string[] TabComplete(string input)
		{
			if (!input.StartsWith("/", StringComparison.OrdinalIgnoreCase))
				return null;

			var data = input
					.Substring(1).Split('"')
					.Select((element, index) => index % 2 == 0  // If even index
						? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)  // Split the item
						: new string[] { element })  // Keep the entire item
					.SelectMany(element => element);

			var cmd = data.Any() ? Command.Create(data.First()) : null;
			if (cmd == null)
			{
				if (!data.Any())
					return Command.Available.ToArray();

				return Command.Available.Where(c => c.StartsWith(data.First(), StringComparison.CurrentCultureIgnoreCase)).ToArray();
			}

			string[] output;
			if (cmd.TabComplete(data.Skip(1).ToString(" "), out output))
				return output;
			return null;
		}

		public static void RunCommand(string command, IEnumerable<string> Args)
		{
			var cmd = Command.Create(command);

			if (cmd == null)
				throw new Exception(string.Format("Command not found: {0}", command));

			cmd.Invoke(Args);
		}

		public static void TextEntry(string Text)
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
				
				RunCommand(data.First().ToLower(), data.Skip(1));
				return;
			}

			throw new NotImplementedException("Messaging not implemented.");
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

				var size = new Size(Console.WindowWidth, Console.WindowHeight) - new Size(1, 0);

				_Root.Size = size;
				_MainPanel.Size = size - new Size(0, 2);
				_StatusPanel.Size = new Size(size.Width, 2);
				_InputPanel.Size = new Size(size.Width, 1);
			}
				
			_Root.Draw();

			_InputBox.Focus();
		}

		public static void Run()
		{
			BuildUI();
			//Connect();
			//Login();

			Console.Title = string.Format("FChat Messenger v{0}", Assembly.GetExecutingAssembly().GetName().Version);

			_Chat.Connection.Endpoint = libflist.Connection.ChatConnection.TestingServerEndpoint;

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