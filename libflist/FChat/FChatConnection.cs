using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using libflist.Connection;
using libflist.Connection.Commands;
using libflist.Connection.Types;
using libflist.Events;
using libflist.JSON.Responses;
using libflist.Util.Converters;
using System.Diagnostics;
using System.Reflection;
using libflist.FChat.Events;
using System.Threading;
using WebSocketSharp;
using libflist.FChat.Util;

namespace libflist.FChat
{
	public sealed class FChatConnection : IDisposable
	{
		public sealed class KnownChannel
		{
			public string ID { get; set; }
			public string Title { get; set; }
			public ChannelMode Mode { get; set; }
			public int UserCount { get; set; }

			public bool Official { get { return ID == Title; } }
		}

		static readonly TimeSpan OFFICIAL_TIMEOUT = TimeSpan.FromMinutes(5);
		static readonly TimeSpan PRIVATE_TIMEOUT = TimeSpan.FromMinutes(1);

		public static readonly Uri LiveServerEndpoint = new Uri("wss://chat.f-list.net:9799");
		public static readonly Uri TestingServerEndpoint = new Uri("wss://chat.f-list.net:8799");
#if DEBUG
		public Uri Endpoint = TestingServerEndpoint;
#else
		public Uri Endpoint = LiveServerEndpoint;
#endif

		DateTime _LastPublicUpdate;
		DateTime _LastPrivateUpdate;
		List<KnownChannel> _OfficialChannels;
		List<KnownChannel> _PrivateChannels;

		List<Channel> _Channels;
		List<Character> _Characters;

		Dictionary<string, Action<Command>> _Handlers;

		readonly ServerVariables _Variables;
		WebSocket _Connection;
		bool _Identified = false;
		string _User;
		string _Character;

		public ServerVariables Variables { get { return _Variables; } }
		public TicketResponse Ticket { get; set; }
		public DateTime TicketTimestamp { get; set; }

		// TODO: Reload if data is stale.
		public IEnumerable<KnownChannel> KnownChannels { get { return _OfficialChannels.Concat(_PrivateChannels); } }

		public IReadOnlyCollection<KnownChannel> OfficialChannels { get { return _OfficialChannels; } }
		public IReadOnlyCollection<KnownChannel> PrivateChannels { get { return _PrivateChannels; } }

		public IEnumerable<Channel> ActiveChannels { get { return _Channels; } }

		public event EventHandler OnConnected;
		public event EventHandler OnDisconnected;
		public event EventHandler OnIdentified;

		public event EventHandler OnError;
		public event EventHandler<CommandEventArgs> OnErrorMessage;
		public event EventHandler<CommandEventArgs> OnRawMessage;
		public event EventHandler<CommandEventArgs> OnSYSMessage;

		public event EventHandler OnOfficialListUpdate;
		public event EventHandler OnPrivateListUpdate;

		public event EventHandler OnOPAdded;
		public event EventHandler OnOPRemoved;

		public event EventHandler<ChannelEntryEventArgs> OnChannelJoin;
		public event EventHandler<ChannelEntryEventArgs> OnChannelLeave;
		public event EventHandler<ChannelEntryEventArgs> OnChannelInfo;

		public event EventHandler OnChannelUserJoin;
		public event EventHandler OnChannelUserLeave;

		public event EventHandler OnChannelUserKicked;
		public event EventHandler OnChannelUserBanned;
		public event EventHandler OnChannelUserUnbanned;
		public event EventHandler OnChannelUserTimedout;

		public event EventHandler OnChannelDescriptionChange;
		public event EventHandler OnChannelModeChange;
		public event EventHandler OnChannelOwnerChange;
		public event EventHandler OnChannelStatusChange;
		public event EventHandler OnChannelTitleChange;

		public event EventHandler OnChannelOPAdded;
		public event EventHandler OnChannelOPRemoved;

		public event EventHandler OnChannelChatMessage;
		public event EventHandler OnChannelLFRPMessage;
		public event EventHandler OnChannelRollMessage;
		public event EventHandler OnChannelSYSMessage;

		public event EventHandler OnCharacterOnline;
		public event EventHandler OnCharacterOffline;

		public event EventHandler OnCharacterKicked;
		public event EventHandler OnCharacterBanned;
		public event EventHandler OnCharacterUnbanned;
		public event EventHandler OnCharacterTimedout;

		public event EventHandler OnCharacterStatusChange;
		public event EventHandler OnCharacterTypingChange;

		public event EventHandler OnCharacterChatMessage;


		public FChatConnection()
		{
			_OfficialChannels = new List<KnownChannel>();
			_PrivateChannels = new List<KnownChannel>();

			_Channels = new List<Channel>();
			_Characters = new List<Character>();
			_Variables = new ServerVariables();

			_Handlers = new Dictionary<string, Action<Command>>();

			_Handlers["!!!"] = (c) => {
				var err = c as Connection.Commands.Meta.FailedReply;
				Debug.WriteLine("Invalid command recieved: {0} - {2}\n{1}", err.CMDToken, err.Data, err.Exception);
			};
			_Handlers["???"] = (c) => {
				var err = c as Connection.Commands.Meta.UnknownReply;
				Debug.WriteLine("Unknown command recieved: {0}\n{1}", err.CMDToken, err.Data);
			};


			_Handlers["CON"] = (c) => {
				var con = c as Connection.Commands.Server.ServerConnected;
				_Variables.SetVariable("__connected", con.ConnectedUsers);
			};
			_Handlers["ERR"] = (c) => {
				var err = c as Connection.Commands.Server.ChatError;
				OnErrorMessage?.Invoke(this, new CommandEventArgs(err));
			};
			_Handlers["HLO"] = (c) =>
			{
				var hlo = c as Connection.Commands.Server.ServerHello;
				/// TODO: Properly report server hello.
				Debug.WriteLine($"Hello: {hlo.Message}");
			};
			_Handlers["IDN"] = (c) => {
				var idn = c as Connection.Commands.Server.ServerIdentify;

				_Identified = true;
				Debug.WriteLine($"Identified as {idn.Character}");
				/// TODO: Handle identifying properly
				OnIdentified?.Invoke(this, EventArgs.Empty);
			};
			_Handlers["PIN"] = (c) => {
				SendCommand(new Connection.Commands.Client.Server.PingCommand());
			};
			_Handlers["SYS"] = (c) => {
				var sys = c as Connection.Commands.Server.SysReply;
				OnSYSMessage?.Invoke(this, new CommandEventArgs(sys));
			};
			_Handlers["UPT"] = (c) => {
				var upt = c as Connection.Commands.Server.ServerUptime;

				_Variables.SetVariable("__boot_time", upt.StartTime);
				_Variables.SetVariable("__users", upt.CurrentUsers);
				_Variables.SetVariable("__channels", upt.Channels);
				_Variables.SetVariable("__connections", upt.AcceptedConnections);
				_Variables.SetVariable("__peak", upt.PeakUsers);
			};
			_Handlers["VAR"] = (c) => {
				var var = c as Connection.Commands.Server.ServerVariable;
				_Variables.SetVariable(var.Name, var.Value);
			};


			_Handlers["ADL"] = (c) => {
				var adl = c as Connection.Commands.Server.ChatOPList;
				/// TODO: Implement OP list
				Debug.WriteLine($"Recieved OP list with {adl.OPs.Length} entries.");
			};
			_Handlers["AOP"] = (c) => {
				var aop = c as Connection.Commands.Server.ChatMakeOP;
				var character = GetCharacter(aop.Character);
				/// TODO: Implement OP list
				OnOPAdded?.Invoke(this, EventArgs.Empty);
			};
			_Handlers["DOP"] = (c) => {
				var dop = c as Connection.Commands.Server.ChatRemoveOP;
				var character = GetCharacter(dop.Character);
				/// TODO: Implement OP list
				OnOPRemoved?.Invoke(this, EventArgs.Empty);
			};


			_Handlers["CHA"] = (c) => {
				var cha = c as Connection.Commands.Server.ChatGetPublicChannels;
				_OfficialChannels.AddRange(cha.Channels.Select(C => new KnownChannel { UserCount = C.Count, ID = C.Name, Title = C.Name, Mode = C.Mode }));
				/// TODO
				OnOfficialListUpdate?.Invoke(this, EventArgs.Empty);
			};
			_Handlers["ORS"] = (c) => {
				var ors = c as Connection.Commands.Server.PrivateChannelListReply;
				_OfficialChannels.AddRange(ors.Channels.Select(C => new KnownChannel { UserCount = C.Count, ID = C.ID, Title = C.Title }));
				/// TODO
				OnOfficialListUpdate?.Invoke(this, EventArgs.Empty);
			};


			_Handlers["FRL"] = (c) => {
				var frl = c as Connection.Commands.Server.FriendListReply;
				/// TODO: Implement friends and bookmarks list
				Debug.WriteLine($"Recieved {frl.FriendsAndBookmarks.Length} friends and bookmarks");
			};
			_Handlers["IGN"] = (c) => {
				var ign = c as Connection.Commands.Server.IgnoreListReply;
				/// TODO: Handle ignores
				switch (ign.Action)
				{
					case Connection.Commands.Server.IgnoreListReply.IgnoreAction.Init:
						Debug.WriteLine($"Initial ignore list received. {ign.Characters.Length} entries.");
						break;

					case Connection.Commands.Server.IgnoreListReply.IgnoreAction.Add:
						Debug.WriteLine($"TODO: Add {ign.Character} to ignore list.");
						break;

					case Connection.Commands.Server.IgnoreListReply.IgnoreAction.Delete:
						Debug.WriteLine($"TODO: Remove {ign.Character} from ignore list.");
						break;
				}
			};
			_Handlers["LIS"] = (c) => {
				var lis = c as Connection.Commands.Server.UserListReply;

				foreach (var character in lis.CharacterData)
				{
					var charObj = GetOrCreateCharacter(character[0]);

					charObj.Gender = JsonEnumConverter.Convert<CharacterGender>(character[1]);
					charObj.Status = JsonEnumConverter.Convert<CharacterStatus>(character[2]);
					charObj.StatusMessage = character[3];
				}
			};


			_Handlers["FLN"] = (c) => {
				var fln = c as Connection.Commands.Server.Character.OfflineReply;
				var character = GetCharacter(fln.Character);
				if (character == null)
				{
					character = new Character(this, fln.Character);

					/// TODO
					OnCharacterOffline?.Invoke(this, EventArgs.Empty);
					return;
				}

				/// TODO
				OnCharacterOffline?.Invoke(this, EventArgs.Empty);

				lock (_Characters)
					_Characters.Remove(character);

				foreach (var chan in _Channels.Where(C => C.Characters.Contains(character)))
					chan.PushCommand(new Connection.Commands.Server.Channel.LeaveReply
					{
						Channel = chan.ID,
						Character = character.Name
					});
			};
			_Handlers["NLN"] = (c) => {
				var nln = c as Connection.Commands.Server.Character.OnlineReply;

				var character = GetOrCreateCharacter(nln.CharacterName);
				character.Gender = nln.Gender;
				character.Status = nln.Status;

				/// TODO
				OnCharacterOnline?.Invoke(this, EventArgs.Empty);
			};
			_Handlers["PRI"] = (c) => {
				var pri = c as Connection.Commands.Server.Character.SendMessageReply;
				var character = GetCharacter(pri.Character);

				character.IsTyping = false;

				/// TODO
				OnCharacterChatMessage?.Invoke(this, EventArgs.Empty);
			};
			_Handlers["STA"] = (c) => {
				var sta = c as Connection.Commands.Server.Character.StatusReply;
				var character = GetCharacter(sta.Character);

				character.Status = sta.Status;
				character.StatusMessage = sta.Message;

				/// TODO
				OnCharacterStatusChange?.Invoke(this, EventArgs.Empty);
			};
			_Handlers["TPN"] = (c) => {
				var tpn = c as Connection.Commands.Server.Character.TypingReply;
				var character = GetCharacter(tpn.Character);

				character.IsTyping = tpn.Status == TypingStatus.Typing;

				OnCharacterTypingChange?.Invoke(this, EventArgs.Empty);
			};
		}

		public void Dispose()
		{
			_Connection.Close();

			_Variables.Clear();
			_Channels = null;
			_Characters = null;

			_Character = null;
			Ticket = null;
			_User = null;
			_Connection = null;
		}

		public Channel GetChannel(string ID)
		{
			lock (_Channels)
				return _Channels.FirstOrDefault(c => c.ID == ID);
		}
		public Channel JoinChannel(string ID)
		{
			lock (_Channels)
			{
				var chan = _Channels.FirstOrDefault(c => c.ID == ID);
				if (chan != null)
					return chan;

				var reply = RequestCommand<Connection.Commands.Server.Channel.JoinReply>(new Connection.Commands.Client.Channel.JoinCommand { Channel = ID });

				chan = new Channel(this, ID, reply.Title);
				_Channels.Add(chan);
				return chan;
			}
		}

		public void SendCommand(Command command)
		{
			if (command.Source != CommandSource.Client)
				throw new ArgumentException("Command source is invalid.", nameof(command));

			lock (_Connection)
				_Connection.Send(command.Serialize());
		}
		public async Task<bool> SendCommandAsync(Command command)
		{
			if (command.Source != CommandSource.Client)
				throw new ArgumentException("Command source is invalid.", nameof(command));

			bool result = false;
			var ev = new AsyncAutoResetEvent();

			_Connection.SendAsync(command.Serialize(), (r) => {
				result = r;
				ev.Set();
			});

			await ev.WaitAsync();

			return result;
		}
		public T RequestCommand<T>(Command query) where T : Command
		{
			var att = query.GetType().GetCustomAttribute<CommandAttribute>();
			if (att.Response != ResponseType.Default || att.ResponseToken == "SYS")
				throw new ArgumentException("Can only use queries with proper response information", nameof(query));

			var ev = new AutoResetEvent(false);

			Command reply = null;
			OnRawMessage += (s, e) =>
			{
				if (e.Command.Token == att.ResponseToken)
				{
					reply = e.Command;
					ev.Set();
				}
			};

			SendCommand(query);
			if (ev.WaitOne(250))
				return (T)reply;
			return null;
		}
		public async Task<T> RequestCommandAsync<T>(Command query) where T : Command
		{
			var att = query.GetType().GetCustomAttribute<CommandAttribute>();
			if (att.Response != ResponseType.Default || att.ResponseToken == "SYS")
				throw new ArgumentException("Can only use queries with proper response information", nameof(query));

			var ev = new AsyncAutoResetEvent();

			Command reply = null;
			OnRawMessage += (s, e) =>
			{
				if (e.Command.Token == att.ResponseToken)
				{
					reply = e.Command;
					ev.Set();
				}
			};

			await SendCommandAsync(query);
			await ev.WaitAsync();

			return (T)reply;
		}


		// TODO: Split connection into several steps;
		//   bool FChat.AquireTicket(string User, string Password);
		//   bool FChat.Ticket.IsValid { get; }
		//   void FChat.Connect();
		//   void FChat.Login(string Character);
		// For instance.

		public void Connect(string User, string Password, bool UseTicket = false)
		{
			if (_Connection != null)
				Disconnect();

			if (User == null)
				throw new ArgumentNullException(nameof(User));

			if (!UseTicket)
			{
				if (Password == null)
					throw new ArgumentNullException(nameof(Password));

				using (var jr = new JSON.Request(JSON.Endpoint.Path.Ticket))
				{
					jr.Data = new Dictionary<string, string> {
						{ "account", User },
						{ "password", Password }
					};

					Ticket = jr.Get<TicketResponse>() as TicketResponse;
					if (!Ticket.Successful)
						return;

					TicketTimestamp = DateTime.Now;
				}
			}

			_User = User;
			_Variables.Clear();

			_Connection = new WebSocket(Endpoint.AbsoluteUri);
			lock (_Connection)
			{
				_Connection.OnClose += _Connection_OnClose;
				_Connection.OnError += _Connection_OnError;
				_Connection.OnMessage += _Connection_OnMessage;
				_Connection.OnOpen += _Connection_OnOpen;

				_Connection.Connect();
			}
		}

		public void Disconnect()
		{
			if (_Connection == null)
				return;

			lock (_Connection)
				_Connection.Close();

			_Connection = null;
			_Identified = false;
		}

		public void Reconnect(bool AutoLogin = true)
		{
			if (Ticket == null || _User == null)
				throw new ArgumentNullException(nameof(Ticket));

			if (DateTime.Now - TicketTimestamp > TimeSpan.FromHours(24))
				throw new ArgumentException("Ticket has timed out, reconnect is not possible");

			_Variables.Clear();
			lock (_Connection)
			{
				_Connection.Connect();

				if (AutoLogin)
					SendCommand(new Connection.Commands.Client.Connection.IdentifyCommand {
						Account = _User,
						Ticket = Ticket.Ticket,
						Character = _Character
					});
			}
		}

		public void Login(string Character)
		{
			if (Character == null)
				throw new ArgumentNullException(nameof(Character));
			if (!Ticket.Characters.Contains(Character))
				throw new ArgumentException("Unknown character specified", nameof(Character));

			lock (_Connection)
			{
				if (_Connection == null)
					throw new Exception("Not connected.");

				if (_Identified)
					return;

				_Character = Character;
				SendCommand(new Connection.Commands.Client.Connection.IdentifyCommand
				{
					Account = _User,
					Ticket = Ticket.Ticket,
					Character = _Character
				});
			}
		}

		public Character LocalCharacter
		{
			get { return GetCharacter(_Character); }
		}

		public Channel GetOrJoinChannel(string ID)
		{
			lock (_Channels)
			{
				var chan = _Channels.FirstOrDefault(c => c.ID.ToLower() == ID.ToLower());
				if (chan != null)
					return chan;

				chan = new Channel(this, ID, ID);
				SendCommand(new Connection.Commands.Client.Channel.JoinCommand
				{
					Channel = ID
				});

				_Channels.Add(chan);
				return chan;
			}
		}
		public Character GetCharacter(string Name)
		{
			lock (_Characters)
			{
				return _Characters.FirstOrDefault(c => c.Name.ToLower() == Name.ToLower());
			}
		}
		public Character GetOrCreateCharacter(string Name)
		{
			lock (_Characters)
			{
				var charac = _Characters.FirstOrDefault(c => c.Name.ToLower() == Name.ToLower());
				if (charac != null)
					return charac;

				charac = new Character(this, Name);

				_Characters.Add(charac);
				return charac;
			}
		}


		private void _Connection_OnOpen(object sender, EventArgs e)
		{

		}

		private void _Connection_OnMessage(object sender, MessageEventArgs e)
		{
			var token = e.Data.Substring(0, 3);
			var data = e.Data.Substring(4);

			var reply = CommandParser.ParseReply(token, data, true);

			_HandleMessage(reply);
		}

		private void _Connection_OnError(object sender, ErrorEventArgs e)
		{
			_HandleMessage(new Connection.Commands.Meta.FailedReply
			{
				Data = e.Message,
				Exception = e.Exception
			});

			Disconnect();
		}

		private void _Connection_OnClose(object sender, CloseEventArgs e)
		{
			Disconnect();

			// TODO: Count reconnect attempts.
			if (!e.WasClean && !string.IsNullOrEmpty(_Character))
				Task.Delay(15000).ContinueWith(_ => Reconnect());
		}

		void _HandleMessage(Command cmd)
		{
			OnRawMessage?.Invoke(this, new CommandEventArgs(cmd));

			if (cmd is Command.IChannelCommand &&
				!string.IsNullOrEmpty((cmd as Command.IChannelCommand).Channel))
			{
				var channel = (cmd as Command.IChannelCommand).Channel;

				Channel channelObj;
				lock (_Channels)
				{
					channelObj = _Channels.FirstOrDefault(c => c.ID == channel);
					if (channelObj == null)
					{
						channelObj = new Channel(this, channel, channel);
						_Channels.Add(channelObj);

						OnChannelJoin?.Invoke(this, new ChannelEntryEventArgs(channelObj, cmd));
					}
				}

				channelObj.PushCommand(cmd);

				if (channelObj.IsDisposed)
				{
					lock (_Channels)
						_Channels.Remove(channelObj);

					OnChannelLeave?.Invoke(this, new ChannelEntryEventArgs(channelObj, cmd));

					return;
				}
			}

			if (_Handlers.ContainsKey(cmd.Token))
				_Handlers[cmd.Token](cmd);
			else
				Debug.WriteLine(string.Format("Unhandled command; {0}", cmd.Token));
		}
	}

}
