The F-Chat communication library of the future.
====

Just my version of a library for making bots and chat clients to F-Chat, no real such libraries exist after all.

Note that this is still a ways away from being ready for prime-time use.

<hr>
FList-sharp
-----------

A C# library for utilizing the F-List JSON API as well as FChat system.

### Working

- JSON API
	- Ticket aquiring.
	- Character data lookup.
- FChat connection.
	- Auth & identification.
	- Message parsing.
		- Still lacking test data for a few message types.
	- Events.
- Chat extension.
	- Channel management.
	- Character management.
	- Basic message routing.


### TODO

- Clean up FChat connection, make it more logical.
	- Connect should be split further into more functions.
- Add in all the admin features.
- Add a BBCode render system with different outputs.
	- HTML, Markdown, Null, etc...
- Implement the last of the JSON API
	- Add V2 endpoint


<hr>
ConsoleMessenger
----------------

### Working

- Basic UI.
- Connection.
- Basic ticket store.
- Chatting.
- Channel joining.


### TODO

- Redo channel buffer to actually calculate message sizes correctly.
	- Internally handle message stream as two separate panels.
- Finish up input handling.
	- Cursor movement, delete, etc...
	- Command history.
	- Tab-completion suggestions.
- Add logging support.
- Add userlist in some way. (Panel on the side?)
- Add channel list window.
- Finish status bar.
	- Log channel activity.
	- Invalidate visual on changes.
- Add hilight system.
- Add tab-completion for nicks in chat.
- Render chat messages properly.
- Be more graceful on exits.
- Improve UI.
	- Only redraw when strictly needed.
	- Use faster redrawing methods where applicable. (ChannelBuffer)
	- Properly clean screen space when redrawing.
	- Fix out-of-bounds assert in graphics when resizing.


### Implemented Commands

- `/connect` - Connect to the network using an earlier retrieved ticket.
- `/connect <user> <password>` - Retrieve a ticket for the given account, then connect to the network.
- `/join <channel>` - Join an official channel.
- `/joinp <channel>` - Join a private channel.
- `/list [public/private]` - Retrieve new lists of public (official) / private channels.
- `/login <character>` - Identify and log in to the network as the given character.
- `/me <message>` - Send an emote message to the current channel.
- `/em <message>` - Send an emote message to the current channel.