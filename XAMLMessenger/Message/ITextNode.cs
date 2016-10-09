using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAMLMessenger.Message
{
    interface ITextNode : INode
    {
        string Text { get; set; }
    }
}
