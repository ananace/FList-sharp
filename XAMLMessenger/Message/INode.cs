using System.Windows.Documents;

namespace XAMLMessenger.Message
{
    interface INode
    {
        string Name { get; }

        Inline ToInline(libflist.FChat.Channel chan);
    }
}
