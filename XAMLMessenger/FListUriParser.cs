using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAMLMessenger
{
    class FListUriParser : HttpStyleUriParser
    {
        public FListUriParser()
        {

        }

        protected override void InitializeAndValidate(Uri uri, out UriFormatException parsingError)
        {
            parsingError = null;
        }
    }
}
