using System;
using libflist.FChat.Commands;

namespace ConsoleMessenger.Commands
{
    [Command("roll", Description = "Rolls dice or spins the bottle")]
    public class Roll : Command
    {
        public void Call(string roll)
        {
            Application.WriteMessage(roll, Application.CurrentChannelBuffer, type: MessageType.Roll);
        }
    }
}

