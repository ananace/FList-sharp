using libflist.Events;
using libflist.FChat.Events;
using libflist.Util.Converters;
using System;
using System.Diagnostics;
using System.Linq;

namespace libflist.FChat
{
	partial class FChatConnection
	{
		private void AddDefaultHandlers()
		{
			// Meta
			_Handlers["!!!"] += (_, c) => {
				var err = c as Commands.Meta.FailedReply;
				Debug.WriteLine("Invalid command recieved: {0} - {2}\n{1}", err.CMDToken, err.Data, err.Exception);
			};
			_Handlers["???"] += (_, c) => {
				var err = c as Commands.Meta.UnknownReply;
				Debug.WriteLine("Unknown command recieved: {0}\n{1}", err.CMDToken, err.Data);
			};


			// Connection related
			_Handlers["CON"] += (_, c) => {
				var con = c as Commands.Server.ServerConnected;
				_Variables.SetVariable("__connected", con.ConnectedUsers);
			};
			_Handlers["ERR"] += (_, c) => {
				var err = c as Commands.Server.ChatError;
				OnErrorMessage?.Invoke(this, new CommandEventArgs(err));
			};
			_Handlers["HLO"] += (_, c) =>
			{
				var hlo = c as Commands.Server.ServerHello;
				// TODO: Properly report server hello.
				Debug.WriteLine($"Hello: {hlo.Message}");
			};
			_Handlers["IDN"] += (_, c) => {
				var idn = c as Commands.Server.ServerIdentify;
				_Identified = true;
				Debug.WriteLine($"Identified as {idn.Character}");
				// TODO: Handle identifying properly
				OnIdentified?.Invoke(this, EventArgs.Empty);
			};
			_Handlers["PIN"] += (_, c) => {
				if (AutoPing)
					SendCommand(new Commands.Client.Server.PingCommand());
			};
			_Handlers["SYS"] += (_, c) => {
				var sys = c as Commands.Server.SysReply;

				if (!string.IsNullOrEmpty(sys.Channel))
					OnChannelSYSMessage?.Invoke(this, new ChannelEntryEventArgs<string>(GetChannel(sys.Channel), sys.Message, sys));
				else
					OnSYSMessage?.Invoke(this, new CommandEventArgs(sys));
			};
			_Handlers["UPT"] += (_, c) => {
				var upt = c as Commands.Server.ServerUptime;

				_Variables.SetVariable("__boot_time", upt.StartTime);
				_Variables.SetVariable("__users", upt.CurrentUsers);
				_Variables.SetVariable("__channels", upt.Channels);
				_Variables.SetVariable("__connections", upt.AcceptedConnections);
				_Variables.SetVariable("__peak", upt.PeakUsers);
			};
			_Handlers["VAR"] += (_, c) => {
				var var = c as Commands.Server.ServerVariable;
				_Variables.SetVariable(var.Name, var.Value);
			};


			// Chat OP related
			_Handlers["ADL"] += (_, c) => {
				var adl = c as Commands.Server.ChatOPList;
				// TODO: Implement OP list
				Debug.WriteLine($"Recieved OP list with {adl.OPs.Length} entries.");
			};
			_Handlers["AOP"] += (_, c) => {
				var aop = c as Commands.Server.ChatMakeOP;
				var character = GetCharacter(aop.Character);
				// TODO: Implement OP list
				OnOPAdded?.Invoke(this, new CharacterEntryEventArgs(character, aop));
			};
			_Handlers["DOP"] += (_, c) => {
				var dop = c as Commands.Server.ChatRemoveOP;
				var character = GetCharacter(dop.Character);
				// TODO: Implement OP list
				OnOPRemoved?.Invoke(this, new CharacterEntryEventArgs(character, dop));
			};


			// Channel list related
			_Handlers["CHA"] += (_, c) => {
				var cha = c as Commands.Server.ChatGetPublicChannels;
				// TODO: Do this properly, sync only changes
				_OfficialChannels.Clear();
				_OfficialChannels.AddRange(cha.Channels.Select(C => new KnownChannel { UserCount = C.Count, ID = C.Name, Title = C.Name, Mode = C.Mode }));
				OnOfficialListUpdate?.Invoke(this, EventArgs.Empty);
			};
			_Handlers["ORS"] += (_, c) => {
				var ors = c as Commands.Server.PrivateChannelListReply;
				// TODO: Do this properly, sync only changes
				_PrivateChannels.Clear();
				_PrivateChannels.AddRange(ors.Channels.Select(C => new KnownChannel { UserCount = C.Count, ID = C.ID, Title = C.Title }));
				OnPrivateListUpdate?.Invoke(this, EventArgs.Empty);
			};


			// Character list related
			_Handlers["FRL"] += (_, c) => {
				var frl = c as Commands.Server.FriendListReply;
				// TODO: Implement friends and bookmarks list
				Debug.WriteLine($"Recieved {frl.FriendsAndBookmarks.Length} friends and bookmarks");

				OnFriendsListUpdate?.Invoke(this, EventArgs.Empty);
			};
			_Handlers["IGN"] += (_, c) => {
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

				// TODO
				OnIgnoreListUpdate?.Invoke(this, EventArgs.Empty);
			};
			_Handlers["LIS"] += (_, c) => {
				var lis = c as Commands.Server.UserListReply;

				foreach (var character in lis.CharacterData)
				{
					var charObj = GetOrCreateCharacter(character[0]);

					charObj.Gender = JsonEnumConverter.Convert<CharacterGender>(character[1]);
					charObj.Status = JsonEnumConverter.Convert<CharacterStatus>(character[2]);
					charObj.StatusMessage = character[3];
				}
			};


			// Channel entry related
			_Handlers["JCH"] += (_, c) => {
				var jch = c as Commands.Server.Channel.JoinReply;

				if (jch.Character.Identity != LocalCharacter.Name)
					OnChannelUserJoin?.Invoke(this, new ChannelUserEntryEventArgs(GetOrCreateChannel(jch.Channel), GetCharacter(jch.Character.Identity), jch));
			};
			_Handlers["LCH"] += (_, c) => {
				var lch = c as Commands.Server.Channel.LeaveReply;

				if (lch.Character == LocalCharacter.Name)
					OnChannelLeave?.Invoke(this, new ChannelEntryEventArgs(GetChannel(lch.Channel), lch));
				else
					OnChannelUserLeave?.Invoke(this, new ChannelUserEntryEventArgs(GetChannel(lch.Channel), GetCharacter(lch.Character), lch));
			};
			_Handlers["ICH"] += (_, c) => {
				var ich = c as Commands.Server.Channel.InitialDataReply;

				OnChannelJoin?.Invoke(this, new ChannelEntryEventArgs(GetOrCreateChannel(ich.Channel), ich));
			};
			_Handlers["CDS"] += (_, c) => {
				var cds = c as Commands.Server.Channel.ChangeDescriptionReply;

				OnChannelDescriptionChange?.Invoke(this, new ChannelEntryEventArgs<string>(GetChannel(cds.Channel), cds.Description, cds));
			};


			// Channel admin related
			_Handlers["CKU"] += (_, c) => {
				var cku = c as Commands.Server.Channel.KickCharacterReply;

				OnChannelUserKicked?.Invoke(this, new ChannelAdminActionEventArgs(GetChannel(cku.Channel), GetCharacter(cku.Character), GetCharacter(cku.OP), cku));
			};
			_Handlers["CBU"] += (_, c) => {
				var cbu = c as Commands.Server.Channel.BanCharacterReply;

				OnChannelUserBanned?.Invoke(this, new ChannelAdminActionEventArgs(GetChannel(cbu.Channel), GetCharacter(cbu.Character), GetCharacter(cbu.OP), cbu));
			};
			_Handlers["CUB"] += (_, c) => {
				var cub = c as Commands.Server.Channel.UnbanCharacterReply;

				OnChannelUserUnbanned?.Invoke(this, new ChannelAdminActionEventArgs(GetChannel(cub.Channel), GetCharacter(cub.Character), GetCharacter(cub.OP), cub));
			};
			_Handlers["CTU"] += (_, c) => {
				var ctu = c as Commands.Server.Channel.TimeoutCharacterReply;

				OnChannelUserTimedout?.Invoke(this, new ChannelAdminActionEventArgs(GetChannel(ctu.Channel), GetCharacter(ctu.Character), GetCharacter(ctu.OP), ctu));
			};


			// Channel status related
			_Handlers["CDS"] += (_, c) => {
				var cds = c as Commands.Server.Channel.ChangeDescriptionReply;

				OnChannelDescriptionChange?.Invoke(this, new ChannelEntryEventArgs<string>(GetChannel(cds.Channel), cds.Description, cds));
			};
			_Handlers["RMO"] += (_, c) => {
				var rmo = c as Commands.Server.Channel.SetModeReply;

				OnChannelModeChange?.Invoke(this, new ChannelEntryEventArgs<ChannelMode>(GetChannel(rmo.Channel), rmo.Mode, rmo));
			};
			_Handlers["CSO"] += (_, c) => {
				var cso = c as Commands.Server.Channel.SetOwnerReply;

				OnChannelOwnerChange?.Invoke(this, new ChannelEntryEventArgs<Character>(GetChannel(cso.Channel), GetCharacter(cso.Character), cso));
			};
			_Handlers["RST"] += (_, c) => {
				var rst = c as Commands.Server.Channel.SetStatusReply;

				OnChannelStatusChange?.Invoke(this, new ChannelEntryEventArgs<ChannelStatus>(GetChannel(rst.Channel), rst.Status, rst));
			};


			// Channel admin related
			_Handlers["COA"] += (_, c) => {
				var coa = c as Commands.Server.Channel.MakeOPReply;

				OnChannelOPAdded?.Invoke(this, new ChannelUserEntryEventArgs(GetChannel(coa.Channel), GetCharacter(coa.Character), coa));
			};
			_Handlers["COR"] += (_, c) => {
				var cor = c as Commands.Server.Channel.RemoveOPReply;

				OnChannelOPRemoved?.Invoke(this, new ChannelUserEntryEventArgs(GetChannel(cor.Channel), GetCharacter(cor.Character), cor));
			};


			// Channel message related
			_Handlers["MSG"] += (_, c) => {
				var msg = c as Commands.Server.Channel.SendMessageReply;

				OnChannelChatMessage?.Invoke(this, new ChannelUserMessageEventArgs(GetChannel(msg.Channel), GetCharacter(msg.Character), msg.Message, msg));
			};
			_Handlers["LRP"] += (_, c) => {
				var lrp = c as Commands.Server.Channel.SendLFRPReply;

				OnChannelLFRPMessage?.Invoke(this, new ChannelUserMessageEventArgs(GetChannel(lrp.Channel), GetCharacter(lrp.Character), lrp.AD, lrp));
			};
			_Handlers["RLL"] += (_, c) => {
				var rll = c as Commands.Server.Channel.SendRollReply;

				OnChannelRollMessage?.Invoke(this, new ChannelUserMessageEventArgs(GetChannel(rll.Channel), GetCharacter(rll.Character), rll.Message, rll));
			};


			// Character entry related
			_Handlers["FLN"] += (_, c) => {
				var fln = c as Commands.Server.Character.OfflineReply;
				var character = GetCharacter(fln.Character);
				if (character == null)
				{
					character = new Character(this, fln.Character);

					OnCharacterOffline?.Invoke(this, new CharacterEntryEventArgs(character, fln));
					return;
				}

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
			_Handlers["NLN"] += (_, c) => {
				var nln = c as Commands.Server.Character.OnlineReply;

				var character = GetOrCreateCharacter(nln.CharacterName);
				character.Gender = nln.Gender;
				character.Status = nln.Status;

				OnCharacterOnline?.Invoke(this, new CharacterEntryEventArgs(character, nln));
			};
			_Handlers["PRI"] += (_, c) => {
				var pri = c as Commands.Server.Character.SendMessageReply;
				var character = GetCharacter(pri.Character);

				character.IsTyping = TypingStatus.Clear;

				OnCharacterChatMessage?.Invoke(this, new CharacterMessageEventArgs(character, pri.Message, pri));
			};
			_Handlers["STA"] += (_, c) => {
				var sta = c as Commands.Server.Character.StatusReply;
				var character = GetCharacter(sta.Character);

				character.Status = sta.Status;
				character.StatusMessage = sta.Message;

				OnCharacterStatusChange?.Invoke(this, new CharacterEntryEventArgs<CharacterStatus>(character, sta.Status, sta));
			};
			_Handlers["TPN"] += (_, c) => {
				var tpn = c as Commands.Server.Character.TypingReply;
				var character = GetCharacter(tpn.Character);

				character.IsTyping = tpn.Status;

				OnCharacterTypingChange?.Invoke(this, new CharacterEntryEventArgs<TypingStatus>(character, tpn.Status, tpn));
			};

			/*
			// Character admin events
			public event EventHandler<AdminActionEventArgs> OnCharacterKicked;
			public event EventHandler<AdminActionEventArgs> OnCharacterBanned;
			public event EventHandler<AdminActionEventArgs> OnCharacterUnbanned;
			public event EventHandler<AdminActionEventArgs> OnCharacterTimedout;
			*/
		}
	}
}
