using libflist.FChat;
using System;
using System.Diagnostics;

namespace ConsoleMessenger.Commands
{
	[Command("connect", Description = "Connect to the F-Chat network")]
	class Connect : Command
	{
		void _Connect()
		{
			Application.Connection.Endpoint = (Application.UseTestEndpoint ? FChatConnection.TestingServerEndpoint : FChatConnection.LiveServerEndpoint);
			Application.Connection.Connect();
		}

		public void Call()
		{
			if (Application.Ticket == null)
				throw new ArgumentException("Can't connect without a valid ticket, try /connect <username> <password>");

			Debug.WriteLine("Attempting connection with earlier ticket.");
			Application.Connection.FListClient.Ticket = Application.Ticket.Ticket;
			_Connect();
		}

		public void Call(string username, string password)
		{
			if (Caller is UI.InputBox)
				(Caller as UI.InputBox).PopHistory(); // Don't store the connect command

			Debug.WriteLine(string.Format("Aquiring ticket for {0}...", username));
			if (!Application.Connection.AquireTicket(username, password))
				throw new Exception("Failed to aquire ticket, invalid login data?");

			if (Application.Connection.FListClient.HasTicket)
				Application.Ticket = new Application.StoredTicket {
					Account = username,
					Ticket = Application.Connection.FListClient.Ticket
				};
			else
				throw new Exception("Aquired ticket but still failed?");

			Debug.WriteLine(string.Format("Aquired ticket for {0}. Available characters are:", username));
			Debug.WriteLine(Application.Connection.FListClient.Ticket.Characters.ToString(", "));

			Debug.WriteLine("Connecting to FChat network...");
			_Connect();
			Debug.WriteLine("Connected.");
		}
	}
}
