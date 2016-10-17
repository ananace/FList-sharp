using libflist.Events;
using libflist.FChat.Commands;
using libflist.FChat.Events;
using libflist.Util.Converters;
using System;
using System.Diagnostics;
using System.Linq;
using libflist.Info;

namespace libflist.FChat
{
	partial class FChatConnection
	{
		private void AddDefaultHandlers()
		{
			// Meta
			_Handlers["!!!"] += (_, c) => {
				var err = c as Server_Meta_Failed;
				Debug.WriteLine("Invalid command recieved: {0} - {2}\n{1}", err.CMDToken, err.Data, err.Exception);
			};
			_Handlers["???"] += (_, c) => {
				var err = c as Server_Meta_Unknown;
				Debug.WriteLine("Unknown command recieved: {0}\n{1}", err.CMDToken, err.Data);
			};


			// Connection related
			_Handlers["CON"] += (_, c) => {
				var con = c as Server_CON_ChatConnectedUsers;
				_Variables.SetVariable("__connected", con.ConnectedUsers);
			};
			_Handlers["ERR"] += (_, c) => {
				var err = c as Server_ERR_ChatError;
				OnErrorMessage?.Invoke(this, new CommandEventArgs(err));
			};
			_Handlers["HLO"] += (_, c) =>
			{
				var hlo = c as Server_HLO_ChatHello;
				// TODO: Properly report server hello.
				Debug.WriteLine($"Hello: {hlo.Message}");
			};
			_Handlers["IDN"] += (_, c) => {
				var idn = c as Server_IDN_ChatIdentify;
				_Identified = true;
				Debug.WriteLine($"Identified as {idn.Character}");
				// TODO: Handle identifying properly
				OnIdentified?.Invoke(this, EventArgs.Empty);
			};
			_Handlers["PIN"] += (_, c) => {
				if (AutoPing)
					SendCommand(new Server_PIN_ChatPing());
			};
			_Handlers["SYS"] += (_, c) => {
				var sys = c as Server_SYS_ChatSYSMessage;

				if (!string.IsNullOrEmpty(sys.Channel))
				{
					var chan = GetChannel(sys.Channel);
					chan.PushCommand(sys);
					OnChannelSYSMessage?.Invoke(this, new ChannelEntryEventArgs<string>(chan, sys.Message, sys));
				}
				else
					OnSYSMessage?.Invoke(this, new CommandEventArgs(sys));
			};
			_Handlers["UPT"] += (_, c) => {
				var upt = c as Server_UPT_ChatUptime;

				_Variables.SetVariable("__boot_time", upt.StartTime);
				_Variables.SetVariable("__users", upt.CurrentUsers);
				_Variables.SetVariable("__channels", upt.Channels);
				_Variables.SetVariable("__connections", upt.AcceptedConnections);
				_Variables.SetVariable("__peak", upt.PeakUsers);

				foreach (var name in new[] { "__boot_time", "__users", "__channels", "__connections", "__peak" })
					OnServerVariableUpdate?.Invoke(this, new ServerVariableEventArgs(name, _Variables[name]));
			};
			_Handlers["VAR"] += (_, c) => {
				var var = c as Server_VAR_ChatVariable;
				_Variables.SetVariable(var.Name, var.Value);

				OnServerVariableUpdate?.Invoke(this, new ServerVariableEventArgs(var.Name, var.Value));
			};


			// Chat OP related
			_Handlers["ADL"] += (_, c) => {
				var adl = c as Server_ADL_ChatListOPs;
                _ChatOPs.Clear();
                _ChatOPs.AddRange(adl.OPs.Select(n => GetCharacter(n)));
				Debug.WriteLine($"Recieved OP list with {adl.OPs.Length} entries.");
			};
			_Handlers["AOP"] += (_, c) => {
				var aop = c as Server_AOP_ChatAddOP;
				var character = GetCharacter(aop.Character);
                _ChatOPs.Add(character);
				OnOPAdded?.Invoke(this, new CharacterEntryEventArgs(character, aop));
			};
			_Handlers["DOP"] += (_, c) => {
				var dop = c as Server_DOP_ChatRemoveOP;
				var character = GetCharacter(dop.Character);
                _ChatOPs.Remove(character);
				OnOPRemoved?.Invoke(this, new CharacterEntryEventArgs(character, dop));
			};


			// Channel list related
			_Handlers["CHA"] += (_, c) => {
				var cha = c as Server_CHA_ChatListPublicChannels;
				// TODO: Do this properly, sync only changes
				_OfficialChannels.Clear();
				_OfficialChannels.AddRange(cha.Channels.Select(C => new KnownChannel { UserCount = C.Count, ID = C.Name, Title = C.Name, Mode = C.Mode }));
				OnOfficialListUpdate?.Invoke(this, EventArgs.Empty);
			};
			_Handlers["ORS"] += (_, c) => {
				var ors = c as Server_ORS_ChatListPrivateChannels;
				// TODO: Do this properly, sync only changes
				_PrivateChannels.Clear();
				_PrivateChannels.AddRange(ors.Channels.Select(C => new KnownChannel { UserCount = C.Count, ID = C.ID, Title = C.Title }));
				OnPrivateListUpdate?.Invoke(this, EventArgs.Empty);
			};


			// Character list related
			_Handlers["FRL"] += (_, c) => {
				var frl = c as Server_FRL_ChatListFriends;

				_Friends.AddRange(frl.FriendsAndBookmarks.Select(ch => GetOrCreateCharacter(ch)));
                _Bookmarks.AddRange(frl.FriendsAndBookmarks.Select(ch => GetOrCreateCharacter(ch)));

				Debug.WriteLine($"Recieved {frl.FriendsAndBookmarks.Length} friends and bookmarks");

				OnFriendsListUpdate?.Invoke(this, EventArgs.Empty);
			};
			_Handlers["IGN"] += (_, c) => {
				var ign = c as Server_IGN_ChatListIgnores;
				// TODO: Handle ignores
				switch (ign.Action)
				{
					case Server_IGN_ChatListIgnores.IgnoreAction.Init:
						Debug.WriteLine($"Initial ignore list received. {ign.Characters.Length} entries.");
						break;

					case Server_IGN_ChatListIgnores.IgnoreAction.Add:
						Debug.WriteLine($"TODO: Add {ign.Character} to ignore list.");
						break;

					case Server_IGN_ChatListIgnores.IgnoreAction.Delete:
						Debug.WriteLine($"TODO: Remove {ign.Character} from ignore list.");
						break;
				}

				// TODO
				OnIgnoreListUpdate?.Invoke(this, EventArgs.Empty);
			};
			_Handlers["LIS"] += (_, c) => {
				var lis = c as Server_LIS_ChatListUsers;

				foreach (var character in lis.CharacterData)
				{
					var charObj = GetOrCreateCharacter(character[0]);

					charObj.Gender = JsonEnumConverter.Convert<Genders>(character[1]);
					charObj.Status = JsonEnumConverter.Convert<CharacterStatus>(character[2]);
					charObj.StatusMessage = character[3];
				}
			};


			// Channel entry related
			_Handlers["JCH"] += (_, c) => {
				var jch = c as Server_JCH_ChannelJoin;

				var chan = GetOrCreateChannel(jch.Channel);
				chan.PushCommand(jch);

				if (jch.Character.Identity != LocalCharacter.Name)
					OnChannelUserJoin?.Invoke(this, new ChannelUserEntryEventArgs(chan, GetCharacter(jch.Character.Identity), jch));
			};
			_Handlers["LCH"] += (_, c) => {
				var lch = c as Server_LCH_ChannelLeave;

				var chan = GetChannel(lch.Channel);
				chan.PushCommand(lch);

				if (lch.Character == LocalCharacter.Name)
					OnChannelLeave?.Invoke(this, new ChannelEntryEventArgs(chan, lch));
				else
					OnChannelUserLeave?.Invoke(this, new ChannelUserEntryEventArgs(chan, GetCharacter(lch.Character), lch));
			};
			_Handlers["ICH"] += (_, c) => {
				var ich = c as Server_ICH_ChannelInitialData;

				var chan = GetOrCreateChannel(ich.Channel);
				chan.PushCommand(ich);

				OnChannelJoin?.Invoke(this, new ChannelEntryEventArgs(chan, ich));
			};


			// Channel admin related
			_Handlers["CKU"] += (_, c) => {
				var cku = c as Server_CKU_ChannelKickCharacter;

				var chan = GetChannel(cku.Channel);
				chan.PushCommand(cku);

				OnChannelUserKicked?.Invoke(this, new ChannelAdminActionEventArgs(chan, GetCharacter(cku.Character), GetCharacter(cku.OP), cku));
			};
			_Handlers["CBU"] += (_, c) => {
				var cbu = c as Server_CBU_ChannelBanCharacter;

				var chan = GetChannel(cbu.Channel);
				chan.PushCommand(cbu);

				OnChannelUserBanned?.Invoke(this, new ChannelAdminActionEventArgs(chan, GetCharacter(cbu.Character), GetCharacter(cbu.OP), cbu));
			};
			_Handlers["CUB"] += (_, c) => {
				var cub = c as Server_CUB_ChannelUnbanCharacter;

				var chan = GetChannel(cub.Channel);
				chan.PushCommand(cub);

				OnChannelUserUnbanned?.Invoke(this, new ChannelAdminActionEventArgs(chan, GetCharacter(cub.Character), GetCharacter(cub.OP), cub));
			};
			_Handlers["CTU"] += (_, c) => {
				var ctu = c as Server_CTU_ChannelTimeoutCharacter;

				var chan = GetChannel(ctu.Channel);
				chan.PushCommand(ctu);

				OnChannelUserTimedout?.Invoke(this, new ChannelAdminActionEventArgs(chan, GetCharacter(ctu.Character), GetCharacter(ctu.OP), ctu));
			};


			// Channel status related
			_Handlers["CDS"] += (_, c) => {
				var cds = c as Server_CDS_ChannelChangeDescription;

				var chan = GetChannel(cds.Channel);
				chan.PushCommand(cds);

				OnChannelDescriptionChange?.Invoke(this, new ChannelEntryEventArgs<string>(chan, cds.Description, cds));
			};
			_Handlers["RMO"] += (_, c) => {
				var rmo = c as Server_RMO_ChannelSetMode;

				var chan = GetChannel(rmo.Channel);
				chan.PushCommand(rmo);

				OnChannelModeChange?.Invoke(this, new ChannelEntryEventArgs<ChannelMode>(chan, rmo.Mode, rmo));
			};
			_Handlers["CSO"] += (_, c) => {
				var cso = c as Server_CSO_ChannelSetOwner;

				var chan = GetChannel(cso.Channel);
				chan.PushCommand(cso);

				OnChannelOwnerChange?.Invoke(this, new ChannelEntryEventArgs<Character>(chan, GetCharacter(cso.Character), cso));
			};
			_Handlers["RST"] += (_, c) => {
				var rst = c as Server_RST_ChannelSetStatus;

				var chan = GetChannel(rst.Channel);
				chan.PushCommand(rst);

				OnChannelStatusChange?.Invoke(this, new ChannelEntryEventArgs<ChannelStatus>(chan, rst.Status, rst));
			};


			// Channel admin related
			_Handlers["COA"] += (_, c) => {
				var coa = c as Server_COA_ChannelMakeOP;

				var chan = GetChannel(coa.Channel);
				chan.PushCommand(coa);

				OnChannelOPAdded?.Invoke(this, new ChannelUserEntryEventArgs(chan, GetCharacter(coa.Character), coa));
			};
			_Handlers["COR"] += (_, c) => {
				var cor = c as Server_COR_ChannelRemoveOP;

				var chan = GetChannel(cor.Channel);
				chan.PushCommand(cor);

				OnChannelOPRemoved?.Invoke(this, new ChannelUserEntryEventArgs(chan, GetCharacter(cor.Character), cor));
			};


			// Channel message related
			_Handlers["MSG"] += (_, c) => {
				var msg = c as Server_MSG_ChannelChatMessage;

				var chan = GetChannel(msg.Channel);
				chan.PushCommand(msg);

				OnChannelChatMessage?.Invoke(this, new ChannelUserMessageEventArgs(chan, GetCharacter(msg.Character), msg.Message, msg));
			};
			_Handlers["LRP"] += (_, c) => {
				var lrp = c as Server_LRP_ChannelLFRPMessage;

				var chan = GetChannel(lrp.Channel);
				chan.PushCommand(lrp);

				OnChannelLFRPMessage?.Invoke(this, new ChannelUserMessageEventArgs(chan, GetCharacter(lrp.Character), lrp.AD, lrp));
			};
			_Handlers["RLL"] += (_, c) => {
				var rll = c as Server_RLL_ChannelRollMessage;

				var chan = GetChannel(rll.Channel);
				chan.PushCommand(rll);

				OnChannelRollMessage?.Invoke(this, new ChannelUserMessageEventArgs(chan, GetCharacter(rll.Character), rll.Message, rll));
			};


			// Character entry related
			_Handlers["FLN"] += (_, c) => {
				var fln = c as Server_FLN_CharacterOffline;
				var character = GetCharacter(fln.Character);
				if (character == null)
				{
					character = new Character(this, new libflist.Character(FListClient, fln.Character));

					OnCharacterOffline?.Invoke(this, new CharacterEntryEventArgs(character, fln));
					return;
				}

				OnCharacterOffline?.Invoke(this, new CharacterEntryEventArgs(character, fln));

				character.Status = CharacterStatus.Offline;

				foreach (var chan in _Channels.Where(C => C.Characters.Contains(character)))
					chan.PushCommand(new Server_LCH_ChannelLeave
					{
						Channel = chan.ID,
						Character = character.Name
					});
			};
			_Handlers["NLN"] += (_, c) => {
				var nln = c as Server_NLN_CharacterOnline;

				var character = GetOrCreateCharacter(nln.CharacterName);
				character.Gender = nln.Gender;
				character.Status = nln.Status;

				OnCharacterOnline?.Invoke(this, new CharacterEntryEventArgs(character, nln));
			};
			_Handlers["PRI"] += (_, c) => {
				var pri = c as Server_PRI_CharacterChatMessage;
				var character = GetCharacter(pri.Character);

				character.IsTyping = TypingStatus.Clear;

				OnCharacterChatMessage?.Invoke(this, new CharacterMessageEventArgs(character, pri.Message, pri));
			};
			_Handlers["STA"] += (_, c) => {
				var sta = c as Server_STA_CharacterStatus;
				var character = GetCharacter(sta.Character);

				character.Status = sta.Status;
				character.StatusMessage = sta.Message;

				OnCharacterStatusChange?.Invoke(this, new CharacterEntryEventArgs<CharacterStatus>(character, sta.Status, sta));
			};
			_Handlers["TPN"] += (_, c) => {
				var tpn = c as Server_TPN_CharacterTypingStatus;
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
