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
- Automatic reconnection.
- Basic settings.


### TODO

- ~~Redo channel buffer to actually calculate message sizes correctly.~~
- ~~Proper scrolling of channel buffer.~~
	- Support scrolling back into log files.
- Multi-line input box.
- Finish up input handling.
	- ~~Cursor movement, delete, etc...~~
	- ~~Command history.~~
	- Tab-completion suggestions.
- ~~Add logging support.~~
- ~~Add userlist in some way. (Panel on the side?)~~ Preliminary implementation through `/who`
- Add channel list window.
- Finish status bar.
	- ~~Log channel activity.~~
- ~~Add hilight system.~~
- Add tab-completion for nicks in chat.
- ~~Render chat messages properly.~~
- Be more graceful on exits.
- Improve UI.
	- Only redraw when strictly needed.
	- Use faster redrawing methods where applicable. (ChannelBuffer?)
	- Properly clean screen space when redrawing.
	- ~~Fix out-of-bounds assert in graphics when resizing.~~


### Implemented Commands

- `/channels` - List all the public channels. (Currently only downloads the list)
- `/clear` - Clears the current chat.
- `/clearall` - Clears all chats.
- `/connect` - Connect to the network using an earlier retrieved ticket.
- `/connect <user> <password>` - Retrieve a ticket for the given account, then connect to the network.
- `/join <channel>` - Join an official channel.
- `/joinp <channel>` - Join a private channel.
- `/list [public/private]` - Retrieve new lists of public (official) / private channels.
- `/login <character>` - Identify and log in to the network as the given character.
- `/me <message>` - Send an emote message to the current channel.
- `/me's <message>` - Send an emote message to the current channel.
- `/em <message>` - Send an emote message to the current channel.
- `/em's <message>` - Send an emote message to the current channel.
- `/priv <character>` - Start a private conversation with the given character.
- `/prooms` - List all the private rooms. (Currently only downloads the list)
- `/roll <dice | 'bottle'>` - Rolls the dice or spins the bottle.
- `/set` - List all the settings along with their current values.
- `/set <setting>` - Retrieves the given setting along with value.
- `/set <setting> <value>` - Set the given setting.

### Settings

- `application.use_test_endpoint` - `bool`, default `true`; should the connection be to test or live?
- `buffer.show_ads` - `bool`, default `true`; Should LFRP Ads be shown?
- `buffer.show_messages` - `bool`, default `true`; Should regular chat messages be shown?
- `buffer.messagetype` - `enum { ANSI, BBCode, Markdown, Plain }`, default `Plain`; How to render the messages?
- `buffer.sys_timeout` - `double?`, default `null`; How long system messages should be shown for? Null disables.
- `buffer.preview_timeout` - `double?`, default `null`; How long preview messages should be shown for? Null disables.
- `buffer.bell_on_highlight` - `bool`, default `false`; Should a bell sound on highlights?
- `buffer.max_messages` - `int`, default `100`; How many messages should be stored in scrollback?
