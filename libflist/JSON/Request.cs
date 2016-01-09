using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace libflist.JSON
{
	public sealed class Request : IDisposable
	{
		public static Endpoint APIVersion { get; set; } = new Endpoints.V1();

		public Endpoint.Path Path { get; set; }
		public Dictionary<string, string> Data { get; set; }

		public Request(Endpoint.Path path)
		{
			Path = path;
		}

		public void Dispose()
		{
			
		}

		public Response Get<T>()
		{
			var task = GetAsync<T>();
			task.Wait();

			return task.Result;
		}

		public async Task<Response> GetAsync<T>()
		{
			using (var http = new HttpClient())
			{
				HttpResponseMessage resp;

				if (Data != null)
					resp = await http.PostAsync(APIVersion.BuildURI(Path), new FormUrlEncodedContent(Data));
				else
					resp = await http.PostAsync(APIVersion.BuildURI(Path), null);

				if (!resp.IsSuccessStatusCode)
				{
					if (APIVersion is Endpoints.V1)
						return Response.Create<T>("{\"error\": \"HTTP Request failed.\"}");
					return Response.Create<T>("{\"error\": \"http_error\", \"error_msg\": \"HTTP Request failed.\"}");
				}

				var data = await resp.Content.ReadAsStringAsync();
				return Response.Create<T>(data);
			}
		}
	}
}

