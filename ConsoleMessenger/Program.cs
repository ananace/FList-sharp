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
		static FChat _Chat;

		public static FChat Connection { get { return _Chat; } }
		public static StoredTicket Ticket { get; set; }

		static void BuildUI()
		{
			_Root = new VerticalPanel();
			_MainPanel = new TitledPanel();
			_StatusPanel = new TitledPanel();
			_InputPanel = new HorizontalPanel();

			_MainPanel.TitleColor = ConsoleColor.Blue;
			_StatusPanel.TitleColor = ConsoleColor.Blue;
			_StatusPanel.Size = new Point(0, 2);

			_StatusPanel.Children.Add(_InputPanel);
			_Root.Children.Add(_MainPanel);
			_Root.Children.Add(_StatusPanel);

			ConsoleHelper.OnConsoleResized += (_, __) => {
				Redraw(true);
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
			string buf = "";

			Console.InputEncoding = Encoding.UTF8;
			Console.OutputEncoding = Encoding.UTF8;

			while (true)
			{
				try
				{
					var key = Console.ReadKey(true);

					switch (key.Key)
					{
					case ConsoleKey.Tab:
						throw new NotImplementedException("TODO: Tab completion.");

					case ConsoleKey.Backspace:
						{
							buf = buf.Substring(0, buf.Length - 1);
							Console.Write('\b');
						} break;

					case ConsoleKey.Enter:
						{
							var cmd = buf;
							buf = "";

							try
							{
								TextEntry(cmd);
							}
							finally
							{
								Console.Write(new string('\b', cmd.Length));
							}
						} break;

					case ConsoleKey.Clear:
						{
							Redraw(true);
							Console.Write(buf);
						} break;

					default:
						{
							if (!char.IsControl(key.KeyChar) &&
								!key.Modifiers.HasFlag(ConsoleModifiers.Alt) &&
								!key.Modifiers.HasFlag(ConsoleModifiers.Control))
							{
								buf += key.KeyChar;
								Console.Write(key.KeyChar);
								break;
							}

							switch(key.KeyChar)
							{
							case '\x04':
								throw new EndOfStreamException();

							case '\f':
								Redraw(true);
								Console.Write(buf);
								break;
							}
						} break;
					}
				}
				catch (EndOfStreamException)
				{
					return;
				}
				catch(AggregateException ex)
				{
					Debug.WriteLine("{0} exception(s) occured running that command;", ex.InnerExceptions.Count);
					foreach (Exception inner in ex.InnerExceptions)
						Debug.WriteLine("!!  {0}: {1}\n{2}\n", inner.GetType().Name, inner.Message, inner.StackTrace);
				}
				catch(Exception ex)
				{
					Debug.WriteLine("{0} occured running that command; {1}\n{2}", ex.GetType().Name, ex.Message, ex.StackTrace);
				}
			}
		}

		public static void RunCommand(string Command, IEnumerable<string> Args)
		{
			throw new NotImplementedException("Commands not yet implemented");
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
				
				var spin = UI.Spinner.Start();

				try
				{
					RunCommand(data.First().ToLower(), data.Skip(1));
				}
				finally
				{
					spin.Stop();
				}
				return;
			}

			throw new NotImplementedException("Messaging not implemented.");
		}

		public static void Redraw(bool full = false)
		{
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

			if (full)
				Console.SetCursorPosition(_InputPanel.DisplayPosition.X, _InputPanel.DisplayPosition.Y);
		}

		public static void Run()
		{
			BuildUI();
			//Connect();
			//Login();

			Console.Title = string.Format("FChat Messenger v{0}", Assembly.GetExecutingAssembly().GetName().Version);

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
					Debug.WriteLine("TICKET] Old ticket has timed out.");
					return null;
				}

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