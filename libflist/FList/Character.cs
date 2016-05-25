namespace libflist.FList
{
	public class Character
	{
		readonly IInternalFListClient _Client;
		public IFListClient Client { get { return _Client; } }

		public string Name { get; internal set; }
		public int ID { get; internal set; }

		internal Character(IInternalFListClient client, string name)
		{
			_Client = client;
			Name = name;
		}
		internal Character(IInternalFListClient client, string name, int id)
		{
			_Client = client;
			Name = name;
			ID = id;
		}
	}
}
