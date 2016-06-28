using System.Collections.Generic;

namespace libCBUI.Controls
{
	public class ControlCollection : List<Control>
	{
		public ControlCollection()
		{
		}

		public ControlCollection(IEnumerable<Control> list)
		{
			AddRange(list);
		}
	}
}
