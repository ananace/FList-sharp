using System;
using System.Collections.Generic;

namespace libflist.JSON.Endpoints
{
	public sealed class V1 : Endpoint
	{
		public V1()
		{
			BaseURI = "https://www.f-list.net/json/";
			Paths = new Dictionary<Path, string> {
				{ Path.Ticket, "getApiTicket.php" },
				{ Path.BookmarkAdd, "api/bookmark-add.php" }
			};
		}
	}
}

