using libflist.Events;
using libflist.FChat.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace libflist.FChat
{
	interface IFChatConnection
	{
		// Server events
		event EventHandler OnConnected;
		event EventHandler OnDisconnected;
		event EventHandler OnIdentified;
		event EventHandler<ServerVariableEventArgs> OnServerVariableUpdate;

		// Message events
		event EventHandler<ErrorEventArgs> OnError;
		event EventHandler<CommandEventArgs> OnErrorMessage;
		event EventHandler<CommandEventArgs> OnRawMessage;
		event EventHandler<CommandEventArgs> OnSYSMessage;
		event EventHandler<CommandEventArgs> OnSendMessage;

		// Channel list events
		event EventHandler OnOfficialListUpdate;
		event EventHandler OnPrivateListUpdate;

		// OP events
		event EventHandler<CharacterEntryEventArgs> OnOPAdded;
		event EventHandler<CharacterEntryEventArgs> OnOPRemoved;

		// Channel entry events
		event EventHandler<ChannelEntryEventArgs> OnChannelJoin;
		event EventHandler<ChannelEntryEventArgs> OnChannelLeave;

		// Channel user entry events
		event EventHandler<ChannelUserEntryEventArgs> OnChannelUserJoin;
		event EventHandler<ChannelUserEntryEventArgs> OnChannelUserLeave;

		// Channel admin events
		event EventHandler<ChannelAdminActionEventArgs> OnChannelUserKicked;
		event EventHandler<ChannelAdminActionEventArgs> OnChannelUserBanned;
		event EventHandler<ChannelAdminActionEventArgs> OnChannelUserUnbanned;
		event EventHandler<ChannelAdminActionEventArgs> OnChannelUserTimedout;

		// Channel status events
		event EventHandler<ChannelEntryEventArgs<string>> OnChannelDescriptionChange;
		event EventHandler<ChannelEntryEventArgs<ChannelMode>> OnChannelModeChange;
		event EventHandler<ChannelEntryEventArgs<Character>> OnChannelOwnerChange;
		event EventHandler<ChannelEntryEventArgs<ChannelStatus>> OnChannelStatusChange;
		// event EventHandler<ChannelEntryEventArgs<string>> OnChannelTitleChange;

		// Channel OP events
		event EventHandler<ChannelUserEntryEventArgs> OnChannelOPAdded;
		event EventHandler<ChannelUserEntryEventArgs> OnChannelOPRemoved;

		// Channel message events
		event EventHandler<ChannelUserMessageEventArgs> OnChannelChatMessage;
		event EventHandler<ChannelUserMessageEventArgs> OnChannelLFRPMessage;
		event EventHandler<ChannelUserMessageEventArgs> OnChannelRollMessage;
		event EventHandler<ChannelEntryEventArgs<string>> OnChannelSYSMessage;

		// Character entry events
		event EventHandler<CharacterEntryEventArgs> OnCharacterOnline;
		event EventHandler<CharacterEntryEventArgs> OnCharacterOffline;

		// Character list events
		event EventHandler OnFriendsListUpdate;
		event EventHandler OnIgnoreListUpdate;

		// Character admin events
		event EventHandler<AdminActionEventArgs> OnCharacterKicked;
		event EventHandler<AdminActionEventArgs> OnCharacterBanned;
		event EventHandler<AdminActionEventArgs> OnCharacterUnbanned;
		event EventHandler<AdminActionEventArgs> OnCharacterTimedout;

		// Character status events
		event EventHandler<CharacterEntryEventArgs<CharacterStatus>> OnCharacterStatusChange;
		event EventHandler<CharacterEntryEventArgs<TypingStatus>> OnCharacterTypingChange;

		// Character message events
		event EventHandler<CharacterMessageEventArgs> OnCharacterChatMessage;


        Character LocalCharacter { get; }
		IEnumerable<Character> Friends { get; }
		IEnumerable<Character> Bookmarks { get; }
		IEnumerable<Character> ChatOPs { get; }

		IEnumerable<Channel> ActiveChannels { get; }

		bool IsConnected { get; }
		bool IsIdentified { get; }



		bool AquireTicket(string User, string Password);
		void Connect();
		// void Connect(string User, string Password/APIKey);
		void Disconnect();
		void Reconnect(bool AutoLogin = true);
		void Login(string Character);

		void SendCommand(Command command);
        T RequestCommand<T>(Command query, int msTimeout = 250) where T : Command;

		Channel GetChannel(string ID);
		Channel GetOrCreateChannel(string ID);
        Channel GetOrJoinChannel(string ID);
		Character GetCharacter(string Name);
		Character GetOrCreateCharacter(string Name);


        Task SendCommandAsync(Command command);
        Task<T> RequestCommandAsync<T>(Command query, int msTimeout = 250) where T : Command;
	}
}
