using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using libflist.Events;
using libflist.FChat.Commands;
using libflist.FChat.Events;
using libflist.Util;
using WebSocketSharp;

namespace libflist.FChat
{
	public sealed partial class FChatConnection : IDisposable
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
        List<Character> _ChatOPs;

		Timer _VariableTimer;

		readonly Dictionary<string, EventHandler<Command>> _Handlers;

		readonly ServerVariables _Variables;
		WebSocket _Connection;
		bool _Identified = false;
		string _Character;

		public IFListClient FListClient { get; set; }

		public bool AutoPing { get; set; }
		public bool AutoReconnect { get; set; }
		public bool AutoUpdate { get; set; }

        public IReadOnlyDictionary<string, EventHandler<Command>> MessageHandlers => _Handlers;
        public ServerVariables Variables => _Variables;

		public IEnumerable<KnownChannel> AllKnownChannels { get { return _OfficialChannels.Concat(_PrivateChannels); } }
		public IReadOnlyCollection<KnownChannel> OfficialChannels
		{
			get
			{
				if (AutoUpdate && DateTime.Now > _LastPublicUpdate + OFFICIAL_TIMEOUT)
				{
					_LastPublicUpdate = DateTime.Now;
					RequestCommand<Server_CHA_ChatListPublicChannels>(new Client_CHA_ChatListOfficialChannels());
					return _OfficialChannels;
				}
				return _OfficialChannels;
			}
		}
		public IReadOnlyCollection<KnownChannel> PrivateChannels
		{
			get
			{
				if (AutoUpdate && DateTime.Now > _LastPrivateUpdate + PRIVATE_TIMEOUT)
				{
					_LastPrivateUpdate = DateTime.Now;
					RequestCommand<Server_ORS_ChatListPrivateChannels>(new Client_ORS_ChatListPrivateChannels());
					return _PrivateChannels;
				}
				return _PrivateChannels;
			}
		}

        public IReadOnlyList<Character> ChatOPs => _ChatOPs;

        public IEnumerable<Channel> ActiveChannels => _Channels;
        public bool IsConnected { get { return _Connection != null && _Connection.ReadyState != WebSocketState.Closed; } }

		// Server events
		public event EventHandler OnConnected;
		public event EventHandler OnDisconnected;
		public event EventHandler OnIdentified;
		public event EventHandler OnServerVariableUpdate;

		// Message events
		public event EventHandler<ErrorEventArgs> OnError;
		public event EventHandler<CommandEventArgs> OnErrorMessage;
		public event EventHandler<CommandEventArgs> OnRawMessage;
		public event EventHandler<CommandEventArgs> OnSYSMessage;

		// Channel list events
		public event EventHandler OnOfficialListUpdate;
		public event EventHandler OnPrivateListUpdate;

		// OP events
		public event EventHandler<CharacterEntryEventArgs> OnOPAdded;
		public event EventHandler<CharacterEntryEventArgs> OnOPRemoved;

		// Channel entry events
		public event EventHandler<ChannelEntryEventArgs> OnChannelJoin;
		public event EventHandler<ChannelEntryEventArgs> OnChannelLeave;

		// Channel user entry events
		public event EventHandler<ChannelUserEntryEventArgs> OnChannelUserJoin;
		public event EventHandler<ChannelUserEntryEventArgs> OnChannelUserLeave;

		// Channel admin events
		public event EventHandler<ChannelAdminActionEventArgs> OnChannelUserKicked;
		public event EventHandler<ChannelAdminActionEventArgs> OnChannelUserBanned;
		public event EventHandler<ChannelAdminActionEventArgs> OnChannelUserUnbanned;
		public event EventHandler<ChannelAdminActionEventArgs> OnChannelUserTimedout;

		// Channel status events
		public event EventHandler<ChannelEntryEventArgs<string>> OnChannelDescriptionChange;
		public event EventHandler<ChannelEntryEventArgs<ChannelMode>> OnChannelModeChange;
		public event EventHandler<ChannelEntryEventArgs<Character>> OnChannelOwnerChange;
		public event EventHandler<ChannelEntryEventArgs<ChannelStatus>> OnChannelStatusChange;
		// public event EventHandler<ChannelEntryEventArgs<string>> OnChannelTitleChange;

		// Channel OP events
		public event EventHandler<ChannelUserEntryEventArgs> OnChannelOPAdded;
		public event EventHandler<ChannelUserEntryEventArgs> OnChannelOPRemoved;

		// Channel message events
		public event EventHandler<ChannelUserMessageEventArgs> OnChannelChatMessage;
		public event EventHandler<ChannelUserMessageEventArgs> OnChannelLFRPMessage;
		public event EventHandler<ChannelUserMessageEventArgs> OnChannelRollMessage;
		public event EventHandler<ChannelEntryEventArgs<string>> OnChannelSYSMessage;

		// Character entry events
		public event EventHandler<CharacterEntryEventArgs> OnCharacterOnline;
		public event EventHandler<CharacterEntryEventArgs> OnCharacterOffline;

		// Character list events
		public event EventHandler OnFriendsListUpdate;
		public event EventHandler OnIgnoreListUpdate;

		// Character admin events
		public event EventHandler<AdminActionEventArgs> OnCharacterKicked;
		public event EventHandler<AdminActionEventArgs> OnCharacterBanned;
		public event EventHandler<AdminActionEventArgs> OnCharacterUnbanned;
		public event EventHandler<AdminActionEventArgs> OnCharacterTimedout;

		// Character status events
		public event EventHandler<CharacterEntryEventArgs<CharacterStatus>> OnCharacterStatusChange;
		public event EventHandler<CharacterEntryEventArgs<TypingStatus>> OnCharacterTypingChange;

		// Character message events
		public event EventHandler<CharacterMessageEventArgs> OnCharacterChatMessage;

		
		public FChatConnection(IFListClient client = null)
		{
			FListClient = client ?? new FListClientV1();

			_OfficialChannels = new List<KnownChannel>();
			_PrivateChannels = new List<KnownChannel>();
            _ChatOPs = new List<Character>();

			_Channels = new List<Channel>();
			_Variables = new ServerVariables();
			_VariableTimer = new Timer((_) => {
				OnServerVariableUpdate?.Invoke(this, EventArgs.Empty);
			});

			_Handlers = new Dictionary<string, EventHandler<Command>>();
			foreach (var token in CommandParser.ImplementedReplies)
				_Handlers.Add(token, null);

			AddDefaultHandlers();
		}

		public void Dispose()
		{
			try
			{
				if (_Connection != null)
					_Connection.Close();
			}
			catch(Exception)
			{ }

			_Variables.Clear();
			_Channels = null;

			_Character = null;
			_Connection = null;
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
		public T RequestCommand<T>(Command query, int msTimeout = 250) where T : Command
		{
			var att = query.GetType().GetCustomAttribute<CommandAttribute>();
			if (att.Response != ResponseType.Default || att.ResponseToken == "SYS")
				throw new ArgumentException("Can only use queries with proper response information", nameof(query));
			var ratt = typeof(T).GetCustomAttribute<ReplyAttribute>();
			if (ratt == null || att.ResponseToken != ratt.Token)
				throw new ArgumentException("Provided respose type is not a valid response to the query");

			var ev = new AutoResetEvent(false);

			Command reply = null;
			var waiter = new EventHandler<Command>((_, e) =>
			{
				reply = e;
				ev.Set();
			});
			_Handlers[ratt.Token] += waiter;

			SendCommand(query);
			var successful = ev.WaitOne(msTimeout);

			_Handlers[ratt.Token] -= waiter;

			return successful ? reply as T : null;
		}
		public async Task<T> RequestCommandAsync<T>(Command query) where T : Command
		{
			var att = query.GetType().GetCustomAttribute<CommandAttribute>();
			if (att.Response != ResponseType.Default || att.ResponseToken == "SYS")
				throw new ArgumentException("Can only use queries with proper response information", nameof(query));
			var ratt = typeof(T).GetCustomAttribute<ReplyAttribute>();
			if (ratt == null || att.ResponseToken != ratt.Token)
				throw new ArgumentException("Provided respose type is not a valid response to the query");
			
			var ev = new AsyncAutoResetEvent();

			Command reply = null;
			var waiter = new EventHandler<Command>((_, e) =>
			{
				reply = e;
				ev.Set();
			});
			_Handlers[ratt.Token] += waiter;

			await SendCommandAsync(query);
			await ev.WaitAsync();

			_Handlers[ratt.Token] -= waiter;

			return reply as T;
		}

		
		public bool AquireTicket(string User, string Password)
		{
			if (FListClient.HasTicket)
				return true;

			var res = FListClient.Authenticate(User, Password);
			res.Wait();

			return res.Result;
		}

		public void Connect()
		{
			if (_Connection != null)
				Disconnect();

			if (!FListClient.HasTicket)
				throw new Exception("You need to aquire a ticket first, in order to connect to the FChat network");
			
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
			{
				try
				{
					_Connection.Close();
				}
				catch (Exception)
				{}
			}

			_Connection = null;
			_Identified = false;
		}

		public void Reconnect(bool AutoLogin = true)
		{
			if (!FListClient.HasTicket)
				throw new Exception("Needs a ticket");

			Disconnect();
			Connect();

			if (AutoLogin)
				SendCommand(new Client_IDN_ChatIdentify {
					Account = FListClient.Ticket.Account,
					Ticket = FListClient.Ticket.Ticket,
					Character = _Character
				});
		}

		public void Login(string Character)
		{
			if (_Connection == null)
				throw new Exception("Not connected.");

			if (_Identified)
				return;
			
			if (Character == null)
				throw new ArgumentNullException(nameof(Character));
			if (!FListClient.Ticket.Characters.Contains(Character))
				throw new ArgumentException("Unknown character specified", nameof(Character));

			lock (_Connection)
			{
				_Character = Character;
				SendCommand(new Client_IDN_ChatIdentify
				{
					Account = FListClient.Ticket.Account,
					Ticket = FListClient.Ticket.Ticket,
					Character = _Character
				});
			}
		}

		public Character LocalCharacter
		{
			get { return GetCharacter(_Character); }
		}

		public Channel GetChannel(string ID)
		{
			lock (_Channels)
				return _Channels.FirstOrDefault(c => c.ID.Equals(ID, StringComparison.CurrentCultureIgnoreCase));
		}
		public Channel GetOrCreateChannel(string ID)
		{
			Channel chan;
			lock (_Channels)
			{
				chan = _Channels.FirstOrDefault(c => c.ID.Equals(ID, StringComparison.CurrentCultureIgnoreCase));
				if (chan != null)
					return chan;

				chan = new Channel(this, ID, ID);
				_Channels.Add(chan);
			}
			return chan;
		}
		public Channel GetOrJoinChannel(string ID)
		{
			lock (_Channels)
			{
				var chan = _Channels.FirstOrDefault(c => c.ID.Equals(ID, StringComparison.CurrentCultureIgnoreCase));
				if (chan != null && chan.Joined)
					return chan;

				var reply = RequestCommand<Server_JCH_ChannelJoin>(new Client_JCH_ChannelJoin { Channel = ID });

				if (chan == null)
				{
					chan = new Channel(this, ID, reply.Title);
					_Channels.Add(chan);
				}
				return chan;
			}
		}
		public Channel GetOrJoinChannelDelayed(string ID)
		{
			Channel chan;
			lock (_Channels)
			{ 
				chan = _Channels.FirstOrDefault(c => c.ID.Equals(ID, StringComparison.CurrentCultureIgnoreCase));
			
				if (chan != null && chan.Joined)
					return chan;

				SendCommand(new Client_JCH_ChannelJoin { Channel = ID });

				if (chan == null)
				{
					chan = new Channel(this, ID, ID);
					_Channels.Add(chan);
				}
				return chan;
			}
		}
		public Channel JoinChannel(string ID)
		{
			lock (_Channels)
			{
				var chan = _Channels.FirstOrDefault(c => c.ID.Equals(ID, StringComparison.CurrentCultureIgnoreCase));
				if (chan != null && chan.Joined)
					return chan;

				var reply = RequestCommand<Server_JCH_ChannelJoin>(new Client_JCH_ChannelJoin { Channel = ID });

				if (chan == null)
				{
					chan = new Channel(this, ID, reply.Title);
					_Channels.Add(chan);
				}
				return chan;
			}
		}
		public Character GetCharacter(string Name)
		{
			lock (FListClient.Characters)
			{
				var character = FListClient.GetOrCreateCharacter(Name);

				if (!(character is Character))
				{
					FListClient.Characters.Remove(character);
					var oldCharacter = character;

					character = new Character(this, oldCharacter);
					FListClient.Characters.Add(character);
				}

				return character as Character;
			}
		}
		public Character GetOrCreateCharacter(string Name)
		{
			lock (FListClient.Characters)
			{
				var character = FListClient.GetOrCreateCharacter(Name);

				if (!(character is Character))
				{
					FListClient.Characters.Remove(character);
					var oldCharacter = character;

					character = new Character(this, oldCharacter);
					FListClient.Characters.Add(character);
				}

				return character as Character;
			}
		}


		void _Connection_OnOpen(object sender, EventArgs e)
		{
			OnConnected?.Invoke(this, e);
		}

		void _Connection_OnMessage(object sender, MessageEventArgs e)
		{
			var token = e.Data.Substring(0, 3);
			var data = e.Data.Length > 3 ? e.Data.Substring(4) : "";

			var reply = CommandParser.ParseReply(token, data, true);

			_Connection_HandleMessage(reply);
		}

		void _Connection_OnError(object sender, ErrorEventArgs e)
		{
			OnError?.Invoke(this, e);
		}

		void _Connection_OnClose(object sender, CloseEventArgs e)
		{
			OnDisconnected?.Invoke(this, e);

			Disconnect();

			// TODO: Count reconnect attempts.
			if (AutoReconnect && !e.WasClean && !string.IsNullOrEmpty(_Character))
				Task.Delay(15000).ContinueWith(_ => Reconnect());
		}

		void _Connection_HandleMessage(Command cmd)
		{
			OnRawMessage?.Invoke(this, new CommandEventArgs(cmd));

			bool preRun = false;
			Channel preChannel = null;
			// Run initial join before calling the handler
			if (cmd.Token == "JCH"
			    && ((preChannel = _Channels
			        .FirstOrDefault(c => c.ID
			                        .Equals((cmd as Server_JCH_ChannelJoin).Channel
			                                , StringComparison.CurrentCultureIgnoreCase))) == null
			        || (preChannel != null && !preChannel.Joined)))
			{
				var chan = GetOrCreateChannel((cmd as Server_JCH_ChannelJoin).Channel);
				chan.PushCommand(cmd);

				preRun = true;
			}

			if (_Handlers.ContainsKey(cmd.Token))
				_Handlers[cmd.Token]?.Invoke(this, cmd);
			else
				Debug.WriteLine(string.Format("Unhandled command; {0}", cmd.Token));
			
			if (!preRun
				&& cmd is Command.IChannelCommand
				&& !string.IsNullOrEmpty((cmd as Command.IChannelCommand).Channel))
			{
				var channel = GetOrCreateChannel((cmd as Command.IChannelCommand).Channel);

				channel.PushCommand(cmd);

				if (channel.IsDisposed)
					_Channels.Remove(channel);
			}
		}
	}

}
