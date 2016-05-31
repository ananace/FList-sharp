﻿using System;
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
			Application.Connection.FListClient.Ticket = Application.Ticket.Ticket;
			Application.Connection.Connect();
		}

		public void Call(string username, string password)
		{
			if (Caller is UI.InputControl)
				(Caller as UI.InputControl).PopHistory(); // Don't store the connect command

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

			Debug.Write("Connecting to FChat network...");
			Application.Connection.Connect();
			Debug.WriteLine(" Done.");
		}
	}
}
