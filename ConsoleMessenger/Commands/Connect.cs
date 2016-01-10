using System;
using System.Diagnostics;

namespace ConsoleMessenger.Commands
{
	[Command("connect", Description = "Connect to the F-Chat network")]
	class Connect : Command
	{
		public void Call()
		{
			if (Application.Ticket == null)
				throw new ArgumentException("Can't connect without a valid ticket, try /connect <username> <password>");

			Debug.WriteLine("Attempting connection with earlier ticket.");
			Application.Connection.Ticket = Application.Ticket.Ticket;
			Application.Connection.TicketTimestamp = Application.Ticket.Timestamp;
			Application.Connection.Connect(Application.Ticket.Account, null, true);
		}

		public void Call(string username, string password)
		{
			Debug.WriteLine(string.Format("Connecting with username {0}", username));
			Application.Connection.Connect(username, password);

			if (Application.Connection.Ticket.Successful)
				Application.Ticket = new Application.StoredTicket
				{
					Account = username,
					Ticket = Application.Connection.Ticket,
					Timestamp = Application.Connection.TicketTimestamp
				};
			else
				throw new Exception(Application.Connection.Ticket.ErrorData ?? Application.Connection.Ticket.Error);
		}
	}
}
