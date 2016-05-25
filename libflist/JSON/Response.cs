﻿using Newtonsoft.Json;

namespace libflist.JSON
{
	public abstract class Response
	{
		[JsonIgnore]
		public bool Successful { get { return string.IsNullOrEmpty(Error) && string.IsNullOrEmpty(ErrorData); } }

		[JsonProperty(PropertyName = "error_msg")]
		public string ErrorData { get; private set; }
		[JsonProperty(PropertyName = "error")]
		public string Error { get; private set; }

		static internal T Create<T>(string Data) where T : Response
		{
			return JsonConvert.DeserializeObject<T>(Data);
		}
	}
}

