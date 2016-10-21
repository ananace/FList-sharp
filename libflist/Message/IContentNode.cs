using System.Collections.Generic;

namespace libflist.Message
{
    public interface IContentNode : INode
    {
        ICollection<INode> Content { get; }
    }
}
