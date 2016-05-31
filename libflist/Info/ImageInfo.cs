using System;

namespace libflist.Info
{

	public sealed class ImageInfo
	{
		public uint Width { get; set; }
		public uint Height { get; set; }
		public Uri Url { get; set; }
		public string Description { get; set; }
	}

}
