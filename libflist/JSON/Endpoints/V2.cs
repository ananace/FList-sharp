using System;
using System.Collections.Generic;

namespace libflist.JSON.Endpoints
{
	public sealed class V2 : Endpoint
	{
		public V2()
		{
			BaseURI = "https://www.f-list.net/api/v2/";
			Paths = new Dictionary<Path, string> {
				{ Path.Ticket, "auth" }
			};
		}
	}
}

