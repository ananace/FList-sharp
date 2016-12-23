using System;
namespace libflist.Message
{
    public interface IRenderer<T>
    {
        T Render(INode node);
    }
}

