using Newtonsoft.Json;

namespace libflist
{
	abstract class Response
	{
		[JsonIgnore]
		public bool Successful { get { return string.IsNullOrEmpty(Error) && string.IsNullOrEmpty(ErrorData); } }

		[JsonProperty(PropertyName = "error_msg")]
		string ErrorData { get; set; }
		[JsonProperty(PropertyName = "error")]
		string Error { get; set; }

		[JsonIgnore]
		public string ErrorMessage { get { return ErrorData ?? Error; } }
		[JsonIgnore]
		public string ErrorCode { get { return ErrorData == null ? "generic" : Error; } }

		static internal T Create<T>(string Data) where T : Response
		{
			return JsonConvert.DeserializeObject<T>(Data);
		}
	}
}
