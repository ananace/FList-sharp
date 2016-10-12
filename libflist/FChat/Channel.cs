using System;
using System.Collections.Generic;
using libflist.Events;
using libflist.FChat.Commands;

namespace libflist.FChat
{
	public enum ChannelMode
	{
		Chat,
		Ads,
		Both
	}

	public enum ChannelStatus
	{
		Public,
		Private
	}

	public sealed class Channel : IDisposable
	{
		List<Character> _Banlist;
		List<Character> _Characters;
		List<Character> _OPs;
		string _OwnerName;

		ChannelStatus _Status;
		FChatConnection _Connection;

		// Channel user entry events
		public event EventHandler<ChannelUserEntryEventArgs> OnUserJoin;
		public event EventHandler<ChannelUserEntryEventArgs> OnUserLeave;

		// Channel admin events
		public event EventHandler<ChannelAdminActionEventArgs> OnUserKicked;
		public event EventHandler<ChannelAdminActionEventArgs> OnUserBanned;
		public event EventHandler<ChannelAdminActionEventArgs> OnUserUnbanned;
		public event EventHandler<ChannelAdminActionEventArgs> OnUserTimedout;

		// Channel status events
		public event EventHandler<ChannelEntryEventArgs<string>> OnDescriptionChange;
		public event EventHandler<ChannelEntryEventArgs<ChannelMode>> OnModeChange;
		public event EventHandler<ChannelEntryEventArgs<Character>> OnOwnerChange;
		public event EventHandler<ChannelEntryEventArgs<ChannelStatus>> OnStatusChange;
		// public event EventHandler<ChannelEntryEventArgs<string>> OnTitleChange;

		// Channel OP events
		public event EventHandler<ChannelUserEntryEventArgs> OnOPAdded;
		public event EventHandler<ChannelUserEntryEventArgs> OnOPRemoved;

		// Channel message events
		public event EventHandler<ChannelUserMessageEventArgs> OnChatMessage;
		public event EventHandler<ChannelUserMessageEventArgs> OnLFRPMessage;
		public event EventHandler<ChannelUserMessageEventArgs> OnRollMessage;
		public event EventHandler<ChannelEntryEventArgs<string>> OnSYSMessage;

		public bool IsDisposed { get; private set; }
		public FChatConnection Connection => _Connection;

		string _ID;
		string _Title;
		string _Description;

		public string Title => _Title;
		public string ID => _ID;
		public string Description
		{
			get
			{
				return _Description;
			}
		 	set
			{
				_Connection.RequestCommand<Server_CDS_ChannelChangeDescription>(new Client_CDS_ChannelSetDescription { Channel = _ID, Description = value });
			}
		}

		public bool Joined { get; private set; }
		public bool Official { get { return Title == ID; } }
		public ChannelMode ChatMode { get; private set; }
		public ChannelStatus PrivacyStatus { get { return Title == ID ? ChannelStatus.Public : _Status; } }

		public Character Owner
		{
			get { return GetCharacter(_OwnerName); }
			set
			{
				_Connection.RequestCommand<Server_CSO_ChannelSetOwner>(new Client_CSO_ChannelSetOwner { Channel = _ID, Character = value.Name });
			}
		}
		public IReadOnlyCollection<Character> OPs { get { return _OPs; } }
		public IReadOnlyCollection<Character> Characters { get { return _Characters; } }
		public IReadOnlyCollection<Character> Banlist { get { return _Banlist; } }

		internal Channel(FChatConnection Connection, string ID, string Title)
		{
			_Connection = Connection;
			_ID = ID;
			_Title = Title;

			_Banlist = new List<Character>();
			_Characters = new List<Character>();
			_OPs = new List<Character>();
			_Status = ChannelStatus.Private;
		}

		public void Dispose()
		{
			_ID = null;
			_Title = null;
			_OwnerName = null;
			_Banlist.Clear();
			_Banlist = null;
			_OPs.Clear();
			_OPs = null;
			_Characters.Clear();
			_Characters = null;
			_Connection = null;

			Joined = false;
			IsDisposed = true;
		}

		public Character GetCharacter(string Name)
		{
			return string.IsNullOrEmpty(Name) 
				? null
				: Connection.GetCharacter(Name);
		}

		public void SendLFRP(string message)
		{
			if (ChatMode == ChannelMode.Chat)
				throw new Exception("Can't send LFRPs in a Chat-only channel.");

			SendCommand(new Client_LRP_ChannelLFRPMessage
			{
				Channel = ID,
				Message = message
			});
		}

		public void SendMessage(string message)
		{
			if (ChatMode == ChannelMode.Ads)
				throw new Exception("Can't send messages in an LFRP-only channel.");

			SendCommand(new Client_MSG_ChannelChatMessage
			{
				Channel = ID,
				Message = message
			});
		}

		public void SendRoll(string roll)
		{
			SendCommand(new Client_RLL_ChannelRollMessage
			{
				Channel = ID,
				Roll = roll
			});
		}

		public void SendCommand(Command cmd)
		{
			Connection.SendCommand(cmd);
		}

		internal void PushCommand(Command cmd)
		{
			if (cmd.Source == CommandSource.Client)
				return;

			switch (cmd.Token)
			{
				case "JCH":
					{
						var jch = cmd as Server_JCH_ChannelJoin;

						var character = Connection.GetOrCreateCharacter(jch.Character.Identity);
						_Characters.Add(character);

						if (!string.IsNullOrEmpty(jch.Title))
						{
							_Title = jch.Title;
							Joined = true;
						}
						else
							OnUserJoin?.Invoke(this, new ChannelUserEntryEventArgs(this, character, jch));
					}
					return;

				case "LCH":
					{
						var lch = cmd as Server_LCH_ChannelLeave;

						var character = GetCharacter(lch.Character);
						_Characters.Remove(character);

						if (character.Name == Connection.LocalCharacter.Name)
							Dispose();
						else
							OnUserLeave?.Invoke(this, new ChannelUserEntryEventArgs(this, character, lch));
					}
					return;

				case "ICH":
					{
						var ich = cmd as Server_ICH_ChannelInitialData;

						ChatMode = ich.Mode;
						Joined = true;

						foreach (var user in ich.Users)
							_Characters.Add(GetCharacter(user.Identity));
					}
					return;

				case "COL":
					{
						var col = cmd as Server_COL_ChannelGetOPs;

						foreach (var op in col.OPs)
							if (!string.IsNullOrWhiteSpace(op))
								_OPs.Add(GetCharacter(op));
					}
					return;

				case "CDS":
					{
						var cds = cmd as Server_CDS_ChannelChangeDescription;

						OnDescriptionChange?.Invoke(this, new ChannelEntryEventArgs<string>(this, cds.Description, cds));
						Description = cds.Description;
					}
					return;

				case "RMO":
					{
						var rmo = cmd as Server_RMO_ChannelSetMode;

						OnModeChange?.Invoke(this, new ChannelEntryEventArgs<ChannelMode>(this, rmo.Mode, rmo));
						ChatMode = rmo.Mode;
					}
					return;

				case "CSO":
					{
						var cso = cmd as Server_CSO_ChannelSetOwner;

						OnOwnerChange?.Invoke(this, new ChannelEntryEventArgs<Character>(this, GetCharacter(cso.Character), cso));
						_OwnerName = cso.Character;
					}
					return;

				case "RST":
					{
						var rst = cmd as Server_RST_ChannelSetStatus;

						OnStatusChange?.Invoke(this, new ChannelEntryEventArgs<ChannelStatus>(this, rst.Status, rst));
						_Status = rst.Status;
					}
					return;

				case "COA":
					{
						var coa = cmd as Server_COA_ChannelMakeOP;

						var character = GetCharacter(coa.Character);
						_OPs.Add(character);

						OnOPAdded?.Invoke(this, new ChannelUserEntryEventArgs(this, character, coa));
					}
					return;

				case "COR":
					{
						var cor = cmd as Server_COR_ChannelRemoveOP;

						var character = GetCharacter(cor.Character);
						_OPs.Remove(character);

						OnOPRemoved?.Invoke(this, new ChannelUserEntryEventArgs(this, character, cor));
					}
					return;

				case "CKU":
					{
						var cku = cmd as Server_CKU_ChannelKickCharacter;

						var character = GetCharacter(cku.Character);
						_Characters.Remove(character);

						OnUserKicked?.Invoke(this, new ChannelAdminActionEventArgs(this, character, GetCharacter(cku.OP), cku));
					}
					return;

				case "CBU":
					{
						var cbu = cmd as Server_CBU_ChannelBanCharacter;

						var character = GetCharacter(cbu.Character);
						_Characters.Remove(character);

						_Banlist.Add(character);

						OnUserBanned?.Invoke(this, new ChannelAdminActionEventArgs(this, character, GetCharacter(cbu.OP), cbu));
					}
					return;

				case "CUB":
					{
						var cub = cmd as Server_CUB_ChannelUnbanCharacter;

						var character = GetCharacter(cub.Character);

						_Banlist.Remove(character);

						OnUserUnbanned?.Invoke(this, new ChannelAdminActionEventArgs(this, character, GetCharacter(cub.OP), cub));
					}
					return;

				case "CTU":
					{
						var ctu = cmd as Server_CTU_ChannelTimeoutCharacter;

						var character = GetCharacter(ctu.Character);
						_Characters.Remove(character);

						OnUserTimedout?.Invoke(this, new ChannelAdminActionEventArgs(this, character, GetCharacter(ctu.OP), ctu));
					}
					return;

				case "MSG":
					{
						var msg = cmd as Server_MSG_ChannelChatMessage;
						OnChatMessage?.Invoke(this, new ChannelUserMessageEventArgs(this, GetCharacter(msg.Character), msg.Message, msg));
					} return;

				case "LRP":
					{
						var lrp = cmd as Server_LRP_ChannelLFRPMessage;
						OnLFRPMessage?.Invoke(this, new ChannelUserMessageEventArgs(this, GetCharacter(lrp.Character), lrp.AD, lrp));
					} return;

				case "RLL":
					{
						var rll = cmd as Server_RLL_ChannelRollMessage;
						OnRollMessage?.Invoke(this, new ChannelUserMessageEventArgs(this, GetCharacter(rll.Character), rll.Message, rll));
					} return;

				case "SYS":
					{
						var sys = cmd as Server_SYS_ChatSYSMessage;
						OnSYSMessage?.Invoke(this, new ChannelEntryEventArgs<string>(this, sys.Message, sys));
					} return;
			}
		}
	}
}

