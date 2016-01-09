using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using libflist.Connection;
using libflist.Connection.Commands;
using libflist.Connection.Types;
using libflist.Events;
using libflist.JSON.Responses;
using libflist.Util.Converters;

namespace libflist
{
	public sealed class FChat : IDisposable
	{
		public sealed class UserAccount : IDisposable
		{
			TicketResponse _Ticket;
			string _User, _Char;

			internal UserAccount(TicketResponse ticket, string user, string charact)
			{
				_Ticket = ticket;
				_User = user;
				_Char = charact;
			}
			public void Dispose()
			{
				
			}

			public string UserName
			{ 
				get { return _User; }
			}

			public string DefaultCharacter
			{
				get { return _Ticket.DefaultCharacter ?? _Ticket.Characters.First(); }
			}
			public IEnumerable<string> Characters
			{
				get { return _Ticket.Characters; }
			}
			public string CurrentCharacter
			{
				get { return _Char; }
			}

			public IEnumerable<Channel> Channels
			{
				get { throw new NotImplementedException(); }
			}
		}

		List<Channel> _Channels;
		List<Character> _Characters;

		ChatConnection _Connection;
		TicketResponse _Ticket;
		DateTime _TicketTime;
		string _User;
		string _Character;

		public ChatConnection Connection { get { return _Connection; } }
		public TicketResponse Ticket { get { return _Ticket; } set { _Ticket = value; } }
		public DateTime TicketTimestamp { get { return _TicketTime; } set { _TicketTime = value; } }

		public event EventHandler<CharacterEntryEventArgs> OnOnline; // JCH
		public event EventHandler<CharacterEntryEventArgs> OnOffline; // LCH

		public event EventHandler<CharacterEntryEventArgs<CharacterStatus>> OnStatusChange;
		public event EventHandler<CharacterEntryEventArgs<TypingStatus>> OnTypingChange;

		public event EventHandler<CharacterEntryEventArgs> OnGivenOP; // COA
		public event EventHandler<CharacterEntryEventArgs> OnRemovedOP; // COR

		/*
		public event EventHandler<AdminActionEventArgs> OnKicked; // CKU
		public event EventHandler<AdminActionEventArgs> OnBanned; // CBU
		public event EventHandler<AdminActionEventArgs> OnUnbanned; // CUB
		public event EventHandler<AdminActionEventArgs> OnTimedout; // CTU
		*/

		public event EventHandler<CharacterMessageEventArgs> OnPrivateMessage; // PRI
		public event EventHandler<ChannelEntryEventArgs<string>> OnErrorMessage; // ERR
		public event EventHandler<ChannelEntryEventArgs<string>> OnSYSMessage; // SYS

		public FChat()
		{
			_Channels = new List<Channel>();
			_Characters = new List<Character>();

			_Connection = new ChatConnection();

			_Connection.OnConnected += _Connection_OnConnected;
			_Connection.OnDisconnected += _Connection_OnDisconnected;
			_Connection.OnIdentified += _Connection_OnIdentified;

			_Connection.OnReceivedCommand += _Connection_OnReceivedCommand;
		}

		public void Dispose()
		{
			_Connection.Disconnect().Wait();

			_Channels = null;
			_Characters = null;

			_Character = null;
			_Ticket = null;
			_User = null;
			_Connection = null;
		}

		public void Connect(string User, string Password, bool UseTicket = false)
		{
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

					_Ticket = jr.Get<TicketResponse>() as TicketResponse;
					if (!_Ticket.Successful)
						return;

					_TicketTime = DateTime.Now;
				}
			}

			_User = User;

			lock (_Connection)
			{
				_Connection.Connect().Wait();
			}
		}

		public void Disconnect()
		{
			lock (_Connection)
				_Connection.Disconnect().Wait();
		}

		public void Reconnect(bool AutoLogin = true)
		{
			if (_Ticket == null || _User == null)
				throw new ArgumentNullException(nameof(_Ticket));

			if (DateTime.Now - _TicketTime > TimeSpan.FromHours(24))
				throw new ArgumentException("Ticket has timed out, reconnect is not possible");

			lock (_Connection)
			{
				_Connection.Connect().Wait();

				if (AutoLogin)
					_Connection.Identify(_User, _Ticket.Ticket, _Character).Wait();
			}
		}

		public void Login(string Character)
		{
			if (Character == null)
				throw new ArgumentNullException(nameof(Character));
			if (!_Ticket.Characters.Contains(Character))
				throw new ArgumentException("Unknown character specified", nameof(Character));

			lock (_Connection)
			{
//				if (!_Connection.Connected)
//					throw new Exception("Not connected.");
				if (_Connection.Identified)
					return;

				_Character = Character;
				_Connection.Identify(_User, _Ticket.Ticket, _Character).Wait();
			}
		}

		public void SendCommand(Command cmd)
		{
			if (cmd.GetType().GetCustomAttribute<ReplyAttribute>() != null)
				throw new ArgumentException("Can't send server replies", nameof(cmd));

			lock (_Connection)
				_Connection.SendCommand(cmd).Wait();
		}


		public UserAccount User
		{
			get { return new UserAccount(_Ticket, _User, _Character); }
		}

		public Character LocalCharacter
		{
			get { return new Character(this, User.CurrentCharacter); }
		}

		public Channel GetOrJoinChannel(string ID)
		{
			lock (_Channels)
			{
				var chan = _Channels.FirstOrDefault(c => c.ID.ToLower() == ID.ToLower());
				if (chan != null)
					return chan;

				chan = new Channel(this, ID, ID);
				SendCommand(new Connection.Commands.Client.Channel.JoinCommand {
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

		void _Connection_OnConnected(object sender, EventArgs e)
		{

		}

		void _Connection_OnDisconnected(object sender, EventArgs e)
		{
			Task.Delay(15000).ContinueWith((arg) => Reconnect());
		}

		void _Connection_OnIdentified(object sender, EventArgs e)
		{
			_Connection.SendCommand(new Connection.Commands.Client.Server.UptimeCommand()).Wait();
		}

		void _Connection_OnReceivedCommand(object sender, Connection.Util.CommandEventArgs e)
		{
			bool handled = false;

			if (e.Command is Command.IChannelCommand &&
			    !string.IsNullOrEmpty((e.Command as Command.IChannelCommand).Channel))
			{
				var channel = (e.Command as Command.IChannelCommand).Channel;

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

				handled |= channelObj.PushCommand(e.Command);

				if (channelObj.IsDisposed)
					lock (_Channels)
						_Channels.Remove(channelObj);
			}

			if (!handled)
			{
				switch (e.Command.Token)
				{
				case "AOP":
					{
						handled = true;

						var aop = e.Command as Connection.Commands.Server.ChatMakeOP;
						var character = GetCharacter(aop.Character);

						if (OnGivenOP != null)
							OnGivenOP(this, new CharacterEntryEventArgs(character, aop));
					} break;

				case "DOP":
					{
						handled = true;

						var dop = e.Command as Connection.Commands.Server.ChatRemoveOP;
						var character = GetCharacter(dop.Character);

						if (OnRemovedOP != null)
							OnRemovedOP(this, new CharacterEntryEventArgs(character, dop));
					} break;

				case "ERR":
					{
						handled = true;

						var err = e.Command as Connection.Commands.Server.ChatError;

						if (OnErrorMessage != null)
							OnErrorMessage(this, new ChannelEntryEventArgs<string>(err.Error, err));
					} break;

				case "FLN":
					{
						handled = true;

						var fln = e.Command as Connection.Commands.Server.Character.OfflineReply;
						var character = GetCharacter(fln.Character);
						if (character == null)
						{
							character = new Character(this, fln.Character);

							if (OnOffline != null)
								OnOffline(this, new CharacterEntryEventArgs(character, fln));
							
							break;
						}

						if (OnOffline != null)
							OnOffline(this, new CharacterEntryEventArgs(character, fln));

						_Characters.Remove(character);

						foreach (var chan in _Channels.Where(c => c.Characters.Contains(character)))
							chan.PushCommand(new Connection.Commands.Server.Channel.LeaveReply {
							Channel = chan.ID,
							Character = {
								Identity = character.Name
							}
						});
					} break;

				case "NLN":
					{
						handled = true;

						var nln = e.Command as Connection.Commands.Server.Character.OnlineReply;

						var character = GetOrCreateCharacter(nln.Character);
						character.Gender = nln.Gender;
						character.Status = nln.Status;

						if (OnOnline != null)
							OnOnline(this, new CharacterEntryEventArgs(character, nln));
					} break;

				case "LIS":
					{
						handled = true;

						var lis = e.Command as Connection.Commands.Server.UserListReply;

						foreach (var character in lis.CharacterData)
						{
							var charObj = GetOrCreateCharacter(character[0]);

							charObj.Gender = JsonEnumConverter.Convert<CharacterGender>(character[1]);
							charObj.Status = JsonEnumConverter.Convert<CharacterStatus>(character[2]);
							charObj.StatusMessage = character[3];
						}
					} break;

				case "PRI":
					{
						handled = true;

						var pri = e.Command as Connection.Commands.Server.Character.SendMessageReply;
						var character = GetCharacter(pri.Character);

						character.IsTyping = false;

						if (OnPrivateMessage != null)
							OnPrivateMessage(this, new CharacterMessageEventArgs(character, pri.Message, pri));
					} break;

				case "STA":
					{
						handled = true;

						var sta = e.Command as Connection.Commands.Server.Character.StatusReply;
						var character = GetCharacter(sta.Character);

						character.Status = sta.Status;
						character.StatusMessage = sta.Message;

						if (OnStatusChange != null)
							OnStatusChange(this, new CharacterEntryEventArgs<CharacterStatus>(character, sta.Status, sta));
					} break;

				case "SYS":
					{
						handled = true;

						var sys = e.Command as Connection.Commands.Server.SysReply;

						if (OnSYSMessage != null)
							OnSYSMessage(this, new ChannelEntryEventArgs<string>(sys.Message, sys));
					} break;

				case "TPN":
					{
						handled = true;

						var tpn = e.Command as Connection.Commands.Server.Character.TypingReply;
						var character = GetCharacter(tpn.Character);

						character.IsTyping = tpn.Status == TypingStatus.Typing;

						if (OnTypingChange != null)
							OnTypingChange(this, new CharacterEntryEventArgs<TypingStatus>(character, tpn.Status, tpn));
					} break;
				}
			}

			if (!handled)
				Console.WriteLine("Unhandled command; {0}", e.Command);
		}
	}
}