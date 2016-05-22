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

		public bool PushCommand(Command cmd)
		{
			if (cmd.Source == CommandSource.Client)
				return false;

			switch (cmd.Token)
			{
				case "JCH":
					{
						var jch = cmd as Commands.Server.Channel.JoinReply;

						var character = new Character(Connection, jch.Character.Identity);
						_Characters.Add(character);

						if (OnJoin != null)
							OnJoin(this, new CharacterEntryEventArgs(this, character, jch));

						if (!string.IsNullOrEmpty(jch.Title))
						{
							var oldTitle = Title;
							Title = jch.Title;

							if (oldTitle != Title && OnTitleChange != null)
								OnTitleChange(this, new ChannelEntryEventArgs<string>(this, Title, jch) { Old = oldTitle });
						}
					}
					return true;

				case "LCH":
					{
						var lch = cmd as Commands.Server.Channel.LeaveReply;

						var character = GetCharacter(lch.Character);
						_Characters.Remove(character);

						if (OnLeave != null)
							OnLeave(this, new CharacterEntryEventArgs(this, character, lch));

						if (character.Name == Connection.User.CurrentCharacter)
							Dispose();
					}
					return true;

				case "ICH":
					{
						var ich = cmd as Commands.Server.Channel.InitialDataReply;

						Mode = ich.Mode;

						foreach (var user in ich.Users)
							_Characters.Add(new Character(Connection, user.Identity));

						if (OnInfo != null)
							OnInfo(this, new ChannelEntryEventArgs(this, ich));
					}
					return true;

				case "COL":
					{
						var col = cmd as Commands.Server.Channel.OPListReply;

						foreach (var op in col.OPs)
							if (!string.IsNullOrWhiteSpace(op))
								_OPs.Add(GetCharacter(op));
					}
					return true;


				case "LRP":
					{
						var lrp = cmd as Commands.Server.Channel.SendLFRPReply;
						var character = GetCharacter(lrp.Character);

						if (OnLFRPMessage != null)
							OnLFRPMessage(this, new CharacterMessageEventArgs(this, character, lrp.AD, lrp));
					}
					return true;
				case "MSG":
					{
						var msg = cmd as Commands.Server.Channel.SendMessageReply;
						var character = GetCharacter(msg.Character);

						if (OnChatMessage != null)
							OnChatMessage(this, new CharacterMessageEventArgs(this, character, msg.Message, msg));
					}
					return true;

				case "RLL":
					{
						var rll = cmd as Commands.Server.Channel.SendRollReply;
						var character = GetCharacter(rll.Character);

						if (OnRollMessage != null)
							OnRollMessage(this, new CharacterMessageEventArgs(this, character, rll.Message, rll));
					}
					return true;

				case "SYS":
					{
						var sys = cmd as Commands.Server.SysReply;

						if (OnSYSMessage != null)
							OnSYSMessage(this, new ChannelEntryEventArgs<string>(this, sys.Message, sys));
					}
					return true;

				case "CDS":
					{
						var cds = cmd as Commands.Server.Channel.ChangeDescriptionReply;

						var oldDesc = Description;
						Description = cds.Description;

						if (OnDescriptionChange != null)
							OnDescriptionChange(this, new ChannelEntryEventArgs<string>(cds.Description, cds) { Old = oldDesc });
					}
					return true;

				case "RMO":
					{
						var rmo = cmd as Commands.Server.Channel.SetModeReply;

						var oldMode = Mode;
						Mode = rmo.Mode;

						if (OnModeChange != null)
							OnModeChange(this, new ChannelEntryEventArgs<ChannelMode>(this, rmo.Mode, rmo) { Old = oldMode });
					}
					return true;

				case "CSO":
					{
						var cso = cmd as Commands.Server.Channel.SetOwnerReply;

						var oldOwner = GetCharacter(_OwnerName);
						var owner = GetCharacter(cso.Character);
                        _OwnerName = cso.Character;

						if (OnOwnerChange != null)
							OnOwnerChange(this, new ChannelEntryEventArgs<Character>(this, owner, cso) { Old = oldOwner });
					}
					return true;

				case "RST":
					{
						var rst = cmd as Commands.Server.Channel.SetStatusReply;

						var oldStatus = _Status;
						_Status = rst.Status;

						if (OnStatusChange != null)
							OnStatusChange(this, new ChannelEntryEventArgs<ChannelStatus>(this, rst.Status, rst) { Old = oldStatus });
					}
					return true;

				case "COA":
					{
						var coa = cmd as Commands.Server.Channel.MakeOPReply;

						var character = GetCharacter(coa.Character);
						_OPs.Add(character);

						if (OnGivenOP != null)
							OnGivenOP(this, new CharacterEntryEventArgs(this, character, coa));
					}
					return true;
				case "COR":
					{
						var cor = cmd as Commands.Server.Channel.RemoveOPReply;

						var character = GetCharacter(cor.Character);
						_OPs.Remove(character);

						if (OnRemovedOP != null)
							OnRemovedOP(this, new CharacterEntryEventArgs(this, character, cor));
					}
					return true;

				case "CKU":
					{
						var cku = cmd as Commands.Server.Channel.KickCharacterReply;

						var character = GetCharacter(cku.Character);
						var admin = GetCharacter(cku.OP);
						_Characters.Remove(character);

						if (_OPs.Contains(character))
							_OPs.Remove(character);

						if (OnKicked != null)
							OnKicked(this, new AdminActionEventArgs(this, character, admin, cku));
					}
					return true;
				case "CBU":
					{
						var cbu = cmd as Commands.Server.Channel.BanCharacterReply;

						var character = GetCharacter(cbu.Character);
						var admin = GetCharacter(cbu.OP);
						_Characters.Remove(character);

						if (_OPs.Contains(character))
							_OPs.Remove(character);

						_Banlist.Add(character);

						if (OnBanned != null)
							OnBanned(this, new AdminActionEventArgs(this, character, admin, cbu));
					}
					return true;
				case "CUB":
					{
						var cub = cmd as Commands.Server.Channel.UnbanCharacterReply;

						var character = GetCharacter(cub.Character);
						var admin = GetCharacter(cub.OP);

						_Banlist.Remove(character);

						if (OnUnbanned != null)
							OnUnbanned(this, new AdminActionEventArgs(this, character, admin, cub));
					}
					return true;
				case "CTU":
					{
						var ctu = cmd as Commands.Server.Channel.TimeoutCharacterReply;

						var character = GetCharacter(ctu.Character);
						var admin = GetCharacter(ctu.OP);
						_Characters.Remove(character);

						if (_OPs.Contains(character))
							_OPs.Remove(character);

						if (OnTimedout != null)
							OnTimedout(this, new AdminActionEventArgs(this, character, admin, ctu));
					}
					return true;
			}

			return false;
		}
	}
}

