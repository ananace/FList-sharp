using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebSocketSharp;
using libflist.Connection.Commands;
using libflist.Connection.Types;
using libflist.Connection.Util;
using System.IO;
using System.Text;

namespace libflist.Connection
{
	public class ChatConnection : IDisposable
	{
		CommandHandler _commands;
		WebSocket _socket;

		public CommandHandler Commands { get { return _commands; } }
		public Uri Endpoint { get; set; }
		public UserRight CurrentUserRight { get; internal set; }

		public bool AutoPing { get; set; }

		public bool Connected { get { return _socket.IsAlive; } }
		public bool Identified { get { return CurrentUserRight > UserRight.Disconnected; } }

		public static readonly Uri LiveServerEndpoint = new Uri("wss://chat.f-list.net:9799");
		public static readonly Uri TestingServerEndpoint = new Uri("wss://chat.f-list.net:8799");

		public event EventHandler<string> PreParseCommand;

		public event EventHandler<CommandEventArgs> OnReceivedCommand;
		public event EventHandler<CommandEventArgs> OnSentCommand;

		public event EventHandler OnConnected;
		public event EventHandler OnDisconnected;
		public event EventHandler OnIdentified;

		public ChatConnection()
		{
			_commands = new CommandHandler(this);
		}

		public void Dispose()
		{
			Disconnect();

			_commands = null;
			Endpoint = null;
		}

		public void Disconnect(CloseStatusCode code = CloseStatusCode.Normal, string reason = null)
		{
			if (_socket == null)
				return;

			var sock = _socket;
			_socket = null;

			if (sock.IsAlive)
				sock.Close(code, reason);
			
			CurrentUserRight = UserRight.Disconnected;
		}

		public void Connect()
		{
			if (Endpoint == null)
				throw new ArgumentNullException(nameof(Endpoint));

			Disconnect(reason: "Reconnecting.");

			CurrentUserRight = UserRight.Disconnected;

			_socket = new WebSocket(Endpoint.AbsoluteUri);

			_socket.OnOpen += (_, __) => {
				if (OnConnected != null)
					OnConnected(this, new EventArgs());
			};
			_socket.OnClose += (_, __) => {
				Disconnect();

				if (OnDisconnected != null)
					OnDisconnected(this, new EventArgs());
			};
			_socket.OnMessage += _socket_OnMessage;

			_socket.Connect();
		}

		public void Identify(string Username, string Ticket, string Character)
		{
			if (Username == null)
				throw new ArgumentNullException(nameof(Username));
			if (Ticket == null)
				throw new ArgumentNullException(nameof(Ticket));
			if (Character == null)
				throw new ArgumentNullException(nameof(Character));

			SendCommand(new Commands.Client.Connection.IdentifyCommand {
				Ticket = Ticket,
				Account = Username,
				Character = Character
			});
		}

		public void SendCommand(Command cmd)
		{
			if (cmd == null)
				throw new ArgumentNullException(nameof(cmd));

			var att = cmd.GetType().GetCustomAttribute<CommandAttribute>();
			if (att == null)
				throw new Exception("Invalid command");
			if (att.MinRight > CurrentUserRight)
				throw new Exception("Not allowed to send command");

			_socket.Send(cmd.ToString());

			if (OnSentCommand != null)
				OnSentCommand(this, new CommandEventArgs(cmd));
		}

		void _socket_OnMessage(object sender, MessageEventArgs arg)
		{
			string token = null;
			string json = null;

			try
			{
				using (var reader = new StringReader(arg.Data))
				{
					char[] data = new char[3];
					reader.Read(data, 0, 3);
					token = new string(data);

					reader.Read();
					json = reader.ReadToEnd();

					if (PreParseCommand != null)
						PreParseCommand(this, string.Format("{0} {1}", token, json));

					var cmd = CommandParser.ParseReply(token, json);
					if (cmd == null)
						return;

					if (OnReceivedCommand != null)
						OnReceivedCommand(this, new CommandEventArgs(cmd));

					if (cmd is Commands.Server.ServerIdentify)
					{
						CurrentUserRight = UserRight.User;
						if (OnIdentified != null)
							OnIdentified(this, new EventArgs());
					}
					else if (AutoPing && cmd is Commands.Server.ServerPing)
					{
						SendCommand(new Commands.Client.Server.PingCommand());
					}
				}
			}
			catch(Exception ex)
			{
				var cmd = new Commands.Meta.FailedReply
				{
					CMDToken = token,
					Data = json,
					Exception = ex
				};

				if (OnReceivedCommand != null)
					OnReceivedCommand(this, new CommandEventArgs(cmd));
			}
		}
	}
}

