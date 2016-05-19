using System;
using System.Collections.Generic;
using System.Linq;

namespace libflist.Connection.Util
{
	public sealed class CommandHandler : IDisposable
	{
		ChatConnection _Conn;
		Dictionary<string, List<Action<CommandEventArgs>>> _Callbacks;

		public CommandHandler(ChatConnection conn)
		{
			_Conn = conn;
			_Callbacks = new Dictionary<string, List<Action<CommandEventArgs>>>();

			_Conn.OnReceivedCommand += OnReceivedCommand;
		}

		public void Dispose()
		{
			_Conn.OnReceivedCommand -= OnReceivedCommand;
			_Conn = null;
		}

		public void On(string Token, Action<CommandEventArgs> cmd)
		{
			if (!_Callbacks.ContainsKey(Token))
				_Callbacks[Token] = new List<Action<CommandEventArgs>>();
			_Callbacks[Token].Add(cmd);
		}

		public void Off(string Token, Action<CommandEventArgs> cmd)
		{
			if (!_Callbacks.ContainsKey(Token))
				return;
			_Callbacks[Token].Remove(cmd);

			if (!_Callbacks[Token].Any())
				_Callbacks.Remove(Token);
		}

		void OnReceivedCommand(object sender, CommandEventArgs e)
		{
			if (!_Callbacks.ContainsKey(e.Command.Token))
				return;

			foreach (var func in _Callbacks[e.Command.Token])
				func(e);
		}
	}
}

