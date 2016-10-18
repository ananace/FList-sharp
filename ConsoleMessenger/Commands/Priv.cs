using System;
using System.Linq;
using ConsoleMessenger.UI.FChat;

namespace ConsoleMessenger.Commands
{
    [Command("priv", Description = "Open a chat with another user")]
    public class Priv : Command
    {
        public void Call(string name)
        {
            var character = Application.Connection.GetOrCreateCharacter(name);
            var chatBuf = Application.Buffers.FirstOrDefault(c => c.Character == character);
            if (chatBuf == null)
            {
                chatBuf = new ChannelBuffer { ChatBuf = new CharacterChatBuffer(character), Title = character.Name };
                Application.Buffers.Add(chatBuf);
            }

            Application.CurrentBuffer = Application.Buffers.IndexOf(chatBuf);
        }
    }
}

