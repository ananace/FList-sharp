using System;
using System.Collections.Generic;
using libflist.FChat.Commands;
using libflist.Connection.Types;
using libflist.Events;

namespace libflist.FChat
{
	public class Channel : IDisposable
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

		public bool Joined { get { return true; } }
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

			_Characters = new List<Character>();
			_OPs = new List<Character>();
			_Status = ChannelStatus.Private;
		}

		public void Dispose()
		{
			ID = null;
			Title = null;
			_OwnerName = null;
			_OPs = null;
			_Characters = null;
			Connection = null;

			IsDisposed = true;
		}

		public Character GetCharacter(string Name)
		{
			return Connection.GetOrCreateCharacter(Name);
		}

		public void SendLFRP(string message)
		{
			if (ChatMode == ChannelMode.Chat)
				throw new Exception("Can't send LFRPs in a Chat-only channel.");

			SendCommand(new Commands.Client.Channel.SendLFRPCommand
			{
				Channel = ID,
				Message = message
			});
		}

		public void SendMessage(string message)
		{
			if (ChatMode == ChannelMode.Ads)
				throw new Exception("Can't send messages in an LFRP-only channel.");

			SendCommand(new Commands.Client.Channel.SendMessageCommand
			{
				Channel = ID,
				Message = message
			});
		}

		public void SendRoll(string roll)
		{
			SendCommand(new Commands.Client.Channel.SendRollCommand
			{
				Channel = ID,
				Roll = roll
			});
		}

		public void SendCommand(Command cmd)
		{
			Connection.SendCommand(cmd);
		}

		public void PushCommand(Command cmd)
		{
			if (cmd.Source == CommandSource.Client)
				return;

			switch (cmd.Token)
			{
			case "JCH":
				{
					var jch = cmd as Commands.Server.Channel.JoinReply;

					var character = new Character(Connection, jch.Character.Identity);
					_Characters.Add(character);

					if (!string.IsNullOrEmpty(jch.Title))
						Title = jch.Title;
				}
				return;

			case "LCH":
				{
					var lch = cmd as Commands.Server.Channel.LeaveReply;

					var character = GetCharacter(lch.Character);
					_Characters.Remove(character);

					if (character.Name == Connection.LocalCharacter.Name)
						Dispose();
				}
				return;

			case "ICH":
				{
					var ich = cmd as Commands.Server.Channel.InitialDataReply;

					ChatMode = ich.Mode;

					foreach (var user in ich.Users)
						_Characters.Add(new Character(Connection, user.Identity));
				}
				return;

			case "COL":
				{
					var col = cmd as Commands.Server.Channel.OPListReply;

					foreach (var op in col.OPs)
						if (!string.IsNullOrWhiteSpace(op))
							_OPs.Add(GetCharacter(op));
				}
				return;

			case "CDS":
				{
					var cds = cmd as Commands.Server.Channel.ChangeDescriptionReply;

					Description = cds.Description;
				}
				return;

			case "RMO":
				{
					var rmo = cmd as Commands.Server.Channel.SetModeReply;

					ChatMode = rmo.Mode;
				}
				return;

			case "CSO":
				{
					var cso = cmd as Commands.Server.Channel.SetOwnerReply;

					_OwnerName = cso.Character;
				}
				return;

			case "RST":
				{
					var rst = cmd as Commands.Server.Channel.SetStatusReply;

					_Status = rst.Status;
				}
				return;

			case "COA":
				{
					var coa = cmd as Commands.Server.Channel.MakeOPReply;

					var character = GetCharacter(coa.Character);
					_OPs.Add(character);
				}
				return;

			case "COR":
				{
					var cor = cmd as Commands.Server.Channel.RemoveOPReply;

					var character = GetCharacter(cor.Character);
					_OPs.Remove(character);
				}
				return;

			case "CKU":
				{
					var cku = cmd as Commands.Server.Channel.KickCharacterReply;

					var character = GetCharacter(cku.Character);
					var admin = GetCharacter(cku.OP);
					_Characters.Remove(character);

					if (_OPs.Contains(character))
						_OPs.Remove(character);
				}
				return;

			case "CBU":
				{
					var cbu = cmd as Commands.Server.Channel.BanCharacterReply;

					var character = GetCharacter(cbu.Character);
					var admin = GetCharacter(cbu.OP);
					_Characters.Remove(character);

					if (_OPs.Contains(character))
						_OPs.Remove(character);

					_Banlist.Add(character);
				}
				return;

			case "CUB":
				{
					var cub = cmd as Commands.Server.Channel.UnbanCharacterReply;

					var character = GetCharacter(cub.Character);

					_Banlist.Remove(character);
				}
				return;

			case "CTU":
				{
					var ctu = cmd as Commands.Server.Channel.TimeoutCharacterReply;

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

