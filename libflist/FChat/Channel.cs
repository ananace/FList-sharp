using System;
using System.Collections.Generic;
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

		public bool IsDisposed { get; private set; }
		public FChatConnection Connection { get; private set; }

		public string Title { get; private set; }
		public string ID { get; private set; }
		public string Description { get; private set; }

		public bool Joined { get; private set; }
		public bool Official { get { return Title == ID; } }
		public ChannelMode ChatMode { get; private set; }
		public ChannelStatus PrivacyStatus { get { return Title == ID ? ChannelStatus.Public : _Status; } }

		public Character Owner { get { return GetCharacter(_OwnerName); } }
		public IReadOnlyCollection<Character> OPs { get { return _OPs; } }
		public IReadOnlyCollection<Character> Characters { get { return _Characters; } }
		public IReadOnlyCollection<Character> Banlist { get { return _Banlist; } }

		internal Channel(FChatConnection Connection, string ID, string Title)
		{
			this.Connection = Connection;
			this.ID = ID;
			this.Title = Title;

			_Banlist = new List<Character>();
			_Characters = new List<Character>();
			_OPs = new List<Character>();
			_Status = ChannelStatus.Private;
		}

		public void Dispose()
		{
			ID = null;
			Title = null;
			_OwnerName = null;
			_Banlist.Clear();
			_Banlist = null;
			_OPs.Clear();
			_OPs = null;
			_Characters.Clear();
			_Characters = null;
			Connection = null;

			Joined = false;
			IsDisposed = true;
		}

		public Character GetCharacter(string Name)
		{
			return string.IsNullOrEmpty(Name) 
				? null
				: Connection.GetOrCreateCharacter(Name);
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
							Title = jch.Title;
							Joined = true;
						}
					}
					return;

				case "LCH":
					{
						var lch = cmd as Server_LCH_ChannelLeave;

						var character = GetCharacter(lch.Character);
						_Characters.Remove(character);

						if (character.Name == Connection.LocalCharacter.Name)
							Dispose();
					}
					return;

				case "ICH":
					{
						var ich = cmd as Server_ICH_ChannelInitialData;

						ChatMode = ich.Mode;
						Joined = true;

						foreach (var user in ich.Users)
							_Characters.Add(new Character(Connection, user.Identity));
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

						Description = cds.Description;
					}
					return;

				case "RMO":
					{
						var rmo = cmd as Server_RMO_ChannelSetMode;

						ChatMode = rmo.Mode;
					}
					return;

				case "CSO":
					{
						var cso = cmd as Server_CSO_ChannelSetOwner;

						_OwnerName = cso.Character;
					}
					return;

				case "RST":
					{
						var rst = cmd as Server_RST_ChannelSetStatus;

						_Status = rst.Status;
					}
					return;

				case "COA":
					{
						var coa = cmd as Server_COA_ChannelMakeOP;

						var character = GetCharacter(coa.Character);
						_OPs.Add(character);
					}
					return;

				case "COR":
					{
						var cor = cmd as Server_COR_ChannelRemoveOP;

						var character = GetCharacter(cor.Character);
						_OPs.Remove(character);
					}
					return;

				case "CKU":
					{
						var cku = cmd as Server_CKU_ChannelKickCharacter;

						var character = GetCharacter(cku.Character);
						_Characters.Remove(character);

						if (_OPs.Contains(character))
							_OPs.Remove(character);
					}
					return;

				case "CBU":
					{
						var cbu = cmd as Server_CBU_ChannelBanCharacter;

						var character = GetCharacter(cbu.Character);
						_Characters.Remove(character);

						if (_OPs.Contains(character))
							_OPs.Remove(character);

						_Banlist.Add(character);
					}
					return;

				case "CUB":
					{
						var cub = cmd as Server_CUB_ChannelUnbanCharacter;

						var character = GetCharacter(cub.Character);

						_Banlist.Remove(character);
					}
					return;

				case "CTU":
					{
						var ctu = cmd as Server_CTU_ChannelTimeoutCharacter;

						var character = GetCharacter(ctu.Character);
						_Characters.Remove(character);

						if (_OPs.Contains(character))
							_OPs.Remove(character);
					}
					return;
			}
		}
	}
}

