using libflist.Util;
using System;

namespace libflist
{

	public class Character
	{
		public IFListClient Client { get; private set; }

		public string Name { get; internal set; }
		public int ID { get; internal set; }

		internal ExpiringLazy<string> _Description;
		public string Description { get { return _Description.Value; } }

		internal Character(IFListClient client, string name) : this(client, name, -1)
		{
		}
		internal Character(IFListClient client, string name, int id)
		{
			Client = client;
			Name = name;
			ID = id;

			_Description = new ExpiringLazy<string>(() =>
			{
				var task = Client.GetDescription(Name);
				task.Wait();

				return task.Result;
			}, TimeSpan.FromMinutes(30));
		}
	}

}
