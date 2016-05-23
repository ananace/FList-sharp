﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using libflist.Connection.Types;
using libflist.Events;
using libflist.FChat.Events;
using libflist.FChat.Util;
using libflist.JSON.Responses;
using libflist.Util.Converters;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using WebSocketSharp;

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

		readonly Dictionary<string, Action<Commands.Command>> _Handlers;

		readonly ServerVariables _Variables;
		WebSocket _Connection;
		bool _Identified = false;
		string _User;
		string _Character;

		public bool AutoPing { get; set; }
		public bool AutoReconnect { get; set; }
		public bool AutoUpdate { get; set; }

		public ServerVariables Variables { get { return _Variables; } }
		public TicketResponse Ticket { get; set; }
		public DateTime TicketTimestamp { get; set; }

		public IEnumerable<KnownChannel> AllKnownChannels { get { return _OfficialChannels.Concat(_PrivateChannels); } }
		public IReadOnlyCollection<KnownChannel> OfficialChannels
		{
			get
			{
				if (AutoUpdate && DateTime.Now > _LastPrivateUpdate + OFFICIAL_TIMEOUT)
				{
					_LastPublicUpdate = DateTime.Now;
					RequestCommand<Commands.Server.ChatGetPublicChannels>(new Commands.Client.Global.GetPublicChannelsCommand());
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
					RequestCommand<Commands.Server.PrivateChannelListReply>(new Commands.Client.Global.GetPrivateChannelsCommand());
					return _PrivateChannels;
				}
				return _PrivateChannels;
			}
		}

		public IEnumerable<Channel> ActiveChannels { get { return _Channels; } }

		// Server events
		public event EventHandler OnConnected;
		public event EventHandler OnDisconnected;
		public event EventHandler OnIdentified;

		// Message events
		public event EventHandler OnError;
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
		public event EventHandler<ChannelEntryEventArgs> OnChannelInfo;

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
		public event EventHandler<ChannelEntryEventArgs<string>> OnChannelTitleChange;

		// Channel OP events
		public event EventHandler<ChannelUserEntryEventArgs> OnChannelOPAdded;
		public event EventHandler<ChannelUserEntryEventArgs> OnChannelOPRemoved;

		// Channel message events
		public event EventHandler<ChannelUserMessageEventArgs> OnChannelChatMessage;
		public event EventHandler<ChannelUserMessageEventArgs> OnChannelLFRPMessage;
		public event EventHandler<ChannelUserEntryEventArgs> OnChannelRollMessage;
		public event EventHandler<ChannelEntryEventArgs<string>> OnChannelSYSMessage;

		// Character entry events
		public event EventHandler<CharacterEntryEventArgs> OnCharacterOnline;
		public event EventHandler<CharacterEntryEventArgs> OnCharacterOffline;

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


		public FChatConnection()
		{
			_OfficialChannels = new List<KnownChannel>();
			_PrivateChannels = new List<KnownChannel>();

			_Channels = new List<Channel>();
			_Characters = new List<Character>();
			_Variables = new ServerVariables();

			_Handlers = new Dictionary<string, Action<Commands.Command>>();

			_Handlers["!!!"] = (c) => {
				var err = c as Commands.Meta.FailedReply;
				Debug.WriteLine("Invalid command recieved: {0} - {2}\n{1}", err.CMDToken, err.Data, err.Exception);
			};
			_Handlers["???"] = (c) => {
				var err = c as Commands.Meta.UnknownReply;
				Debug.WriteLine("Unknown command recieved: {0}\n{1}", err.CMDToken, err.Data);
			};


			_Handlers["CON"] = (c) => {
				var con = c as Commands.Server.ServerConnected;
				_Variables.SetVariable("__connected", con.ConnectedUsers);
			};
			_Handlers["ERR"] = (c) => {
				var err = c as Commands.Server.ChatError;
				OnErrorMessage?.Invoke(this, new CommandEventArgs(err));
			};
			_Handlers["HLO"] = (c) =>
			{
				var hlo = c as Commands.Server.ServerHello;
				// TODO: Properly report server hello.
				Debug.WriteLine($"Hello: {hlo.Message}");
			};
			_Handlers["IDN"] = (c) => {
				var idn = c as Commands.Server.ServerIdentify;
				_Identified = true;
				Debug.WriteLine($"Identified as {idn.Character}");
				// TODO: Handle identifying properly
				OnIdentified?.Invoke(this, EventArgs.Empty);
			};
			_Handlers["PIN"] = (c) => {
				if (AutoPing)	
					SendCommand(new Commands.Client.Server.PingCommand());
			};
			_Handlers["SYS"] = (c) => {
				var sys = c as Commands.Server.SysReply;
				OnSYSMessage?.Invoke(this, new CommandEventArgs(sys));
			};
			_Handlers["UPT"] = (c) => {
				var upt = c as Commands.Server.ServerUptime;

				_Variables.SetVariable("__boot_time", upt.StartTime);
				_Variables.SetVariable("__users", upt.CurrentUsers);
				_Variables.SetVariable("__channels", upt.Channels);
				_Variables.SetVariable("__connections", upt.AcceptedConnections);
				_Variables.SetVariable("__peak", upt.PeakUsers);
			};
			_Handlers["VAR"] = (c) => {
				var var = c as Commands.Server.ServerVariable;
				_Variables.SetVariable(var.Name, var.Value);
			};


			_Handlers["ADL"] = (c) => {
				var adl = c as Commands.Server.ChatOPList;
				// TODO: Implement OP list
				Debug.WriteLine($"Recieved OP list with {adl.OPs.Length} entries.");
			};
			_Handlers["AOP"] = (c) => {
				var aop = c as Commands.Server.ChatMakeOP;
				var character = GetCharacter(aop.Character);
				// TODO: Implement OP list
				OnOPAdded?.Invoke(this, new CharacterEntryEventArgs(character, aop));
			};
			_Handlers["DOP"] = (c) => {
				var dop = c as Commands.Server.ChatRemoveOP;
				var character = GetCharacter(dop.Character);
				// TODO: Implement OP list
				OnOPRemoved?.Invoke(this, new CharacterEntryEventArgs(character, dop));
			};


			_Handlers["CHA"] = (c) => {
				var cha = c as Commands.Server.ChatGetPublicChannels;
				// TODO: Do this properly, sync only changes
				_OfficialChannels.Clear();
				_OfficialChannels.AddRange(cha.Channels.Select(C => new KnownChannel { UserCount = C.Count, ID = C.Name, Title = C.Name, Mode = C.Mode }));
				OnOfficialListUpdate?.Invoke(this, EventArgs.Empty);
			};
			_Handlers["ORS"] = (c) => {
				var ors = c as Commands.Server.PrivateChannelListReply;
				// TODO: Do this properly, sync only changes
				_PrivateChannels.Clear();
				_PrivateChannels.AddRange(ors.Channels.Select(C => new KnownChannel { UserCount = C.Count, ID = C.ID, Title = C.Title }));
				OnPrivateListUpdate?.Invoke(this, EventArgs.Empty);
			};


			_Handlers["FRL"] = (c) => {
				var frl = c as Commands.Server.FriendListReply;
				// TODO: Implement friends and bookmarks list
				Debug.WriteLine($"Recieved {frl.FriendsAndBookmarks.Length} friends and bookmarks");
			};
			_Handlers["IGN"] = (c) => {
				var ign = c as Commands.Server.IgnoreListReply;
				// TODO: Handle ignores
				switch (ign.Action)
				{
					case Commands.Server.IgnoreListReply.IgnoreAction.Init:
						Debug.WriteLine($"Initial ignore list received. {ign.Characters.Length} entries.");
						break;

					case Commands.Server.IgnoreListReply.IgnoreAction.Add:
						Debug.WriteLine($"TODO: Add {ign.Character} to ignore list.");
						break;

					case Commands.Server.IgnoreListReply.IgnoreAction.Delete:
						Debug.WriteLine($"TODO: Remove {ign.Character} from ignore list.");
						break;
				}
			};
			_Handlers["LIS"] = (c) => {
				var lis = c as Commands.Server.UserListReply;

				foreach (var character in lis.CharacterData)
				{
					var charObj = GetOrCreateCharacter(character[0]);

					charObj.Gender = JsonEnumConverter.Convert<CharacterGender>(character[1]);
					charObj.Status = JsonEnumConverter.Convert<CharacterStatus>(character[2]);
					charObj.StatusMessage = character[3];
				}
			};


			_Handlers["FLN"] = (c) => {
				var fln = c as Commands.Server.Character.OfflineReply;
				var character = GetCharacter(fln.Character);
				if (character == null)
				{
					character = new Character(this, fln.Character);

					// TODO
					OnCharacterOffline?.Invoke(this, new CharacterEntryEventArgs(character, fln));
					return;
				}

				// TODO
				OnCharacterOffline?.Invoke(this, new CharacterEntryEventArgs(character, fln));

				lock (_Characters)
					_Characters.Remove(character);

				foreach (var chan in _Channels.Where(C => C.Characters.Contains(character)))
					chan.PushCommand(new Commands.Server.Channel.LeaveReply
					{
						Channel = chan.ID,
						Character = character.Name
					});
			};
			_Handlers["NLN"] = (c) => {
				var nln = c as Commands.Server.Character.OnlineReply;

				var character = GetOrCreateCharacter(nln.CharacterName);
				character.Gender = nln.Gender;
				character.Status = nln.Status;

				OnCharacterOnline?.Invoke(this, new CharacterEntryEventArgs(character, nln));
			};
			_Handlers["PRI"] = (c) => {
				var pri = c as Commands.Server.Character.SendMessageReply;
				var character = GetCharacter(pri.Character);

				character.IsTyping = false;

				OnCharacterChatMessage?.Invoke(this, new CharacterMessageEventArgs(character, pri.Message, pri));
			};
			_Handlers["STA"] = (c) => {
				var sta = c as Commands.Server.Character.StatusReply;
				var character = GetCharacter(sta.Character);

				character.Status = sta.Status;
				character.StatusMessage = sta.Message;

				OnCharacterStatusChange?.Invoke(this, new CharacterEntryEventArgs<CharacterStatus>(character, sta.Status, sta));
			};
			_Handlers["TPN"] = (c) => {
				var tpn = c as Commands.Server.Character.TypingReply;
				var character = GetCharacter(tpn.Character);

				character.IsTyping = tpn.Status == TypingStatus.Typing;

				OnCharacterTypingChange?.Invoke(this, new CharacterEntryEventArgs<TypingStatus>(character, tpn.Status, tpn));
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

				var reply = RequestCommand<Commands.Server.Channel.JoinReply>(new Commands.Client.Channel.JoinCommand { Channel = ID });

				chan = new Channel(this, ID, reply.Title);
				_Channels.Add(chan);
				return chan;
			}
		}

		public void SendCommand(Commands.Command command)
		{
			if (command.Source != Commands.CommandSource.Client)
				throw new ArgumentException("Command source is invalid.", nameof(command));

			lock (_Connection)
				_Connection.Send(command.Serialize());
		}
		public async Task<bool> SendCommandAsync(Commands.Command command)
		{
			if (command.Source != Commands.CommandSource.Client)
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
		public T RequestCommand<T>(Commands.Command query) where T : Commands.Command
		{
			var att = query.GetType().GetCustomAttribute<Commands.CommandAttribute>();
			if (att.Response != ResponseType.Default || att.ResponseToken == "SYS")
				throw new ArgumentException("Can only use queries with proper response information", nameof(query));
			var ratt = typeof(T).GetCustomAttribute<Commands.ReplyAttribute>();
			if (ratt == null || att.ResponseToken != ratt.Token)
				throw new ArgumentException("Provided respose type is not a valid response to the query");

			var ev = new AutoResetEvent(false);

			Commands.Command reply = null;
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
		public async Task<T> RequestCommandAsync<T>(Commands.Command query) where T : Commands.Command
		{
			var att = query.GetType().GetCustomAttribute<Commands.CommandAttribute>();
			if (att.Response != ResponseType.Default || att.ResponseToken == "SYS")
				throw new ArgumentException("Can only use queries with proper response information", nameof(query));
			var ratt = typeof(T).GetCustomAttribute<Commands.ReplyAttribute>();
			if (ratt == null || att.ResponseToken != ratt.Token)
				throw new ArgumentException("Provided respose type is not a valid response to the query");
			
			var ev = new AsyncAutoResetEvent();

			Commands.Command reply = null;
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
			if (Ticket == null || _User == null)
				throw new ArgumentNullException(nameof(Ticket));

			if (DateTime.Now - TicketTimestamp > TimeSpan.FromHours(24))
				throw new ArgumentException("Ticket has timed out, reconnect is not possible");

			_Variables.Clear();
			lock (_Connection)
			{
				_Connection.Connect();

				if (AutoLogin)
					SendCommand(new Commands.Client.Connection.IdentifyCommand {
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
				SendCommand(new Commands.Client.Connection.IdentifyCommand
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
				SendCommand(new Commands.Client.Channel.JoinCommand
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

			var reply = Commands.CommandParser.ParseReply(token, data, true);

			_HandleMessage(reply);
		}

		private void _Connection_OnError(object sender, ErrorEventArgs e)
		{
			_HandleMessage(new Commands.Meta.FailedReply
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
			if (AutoReconnect && !e.WasClean && !string.IsNullOrEmpty(_Character))
				Task.Delay(15000).ContinueWith(_ => Reconnect());
		}

		void _HandleMessage(Commands.Command cmd)
		{
			OnRawMessage?.Invoke(this, new CommandEventArgs(cmd));

			if (cmd is Commands.Command.IChannelCommand &&
				!string.IsNullOrEmpty((cmd as Commands.Command.IChannelCommand).Channel))
			{
				var channel = (cmd as Commands.Command.IChannelCommand).Channel;

				Channel channelObj;
				lock (_Channels)
				{
					channelObj = _Channels.FirstOrDefault(c => c.ID == channel);
					if (channelObj == null)
					{
						channelObj = new Channel(this, channel, channel);
						_Channels.Add(channelObj);
					}
				}

				channelObj.PushCommand(cmd);

				if (channelObj.IsDisposed)
				{
					lock (_Channels)
						_Channels.Remove(channelObj);
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