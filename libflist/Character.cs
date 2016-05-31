using System;
using System.Collections.Generic;
using libflist.Util;

namespace libflist
{

	public class Character
	{

		static readonly TimeSpan TIMEOUT = TimeSpan.FromHours(1);

		public IFListClient Client { get; private set; }

		public string Name { get; internal set; }
		public int ID { get; internal set; }

		internal ExpiringLazy<string> _Description;
		internal ExpiringLazy<Info.ProfileInfo> _ProfileInfo;
		internal ExpiringLazy<List<Info.ImageInfo>> _Images;
		internal ExpiringLazy<Dictionary<Info.KinkInfo, Info.KinkChoice>> _Kinks;

		/// <summary>
		/// Character description block, with BBCode
		/// </summary>
		public string Description { get { return _Description.Value; } }
		/// <summary>
		/// Character information block
		/// </summary>
		public Info.ProfileInfo ProfileInfo { get { return _ProfileInfo.Value; } }
		/// <summary>
		/// Character images
		/// </summary>
		public IReadOnlyCollection<Info.ImageInfo> Images { get { return _Images.Value; } }
		/// <summary>
		/// Character kinks
		/// </summary>
		public IReadOnlyDictionary<Info.KinkInfo, Info.KinkChoice> Kinks { get { return _Kinks.Value; } }

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
			}, TIMEOUT);
			_ProfileInfo = new ExpiringLazy<Info.ProfileInfo>(() =>
			{
				var task = Client.GetInfo(Name);
				task.Wait();

				return task.Result;
			}, TIMEOUT);
			_Images = new ExpiringLazy<List<Info.ImageInfo>>(() =>
			{
				var task = Client.GetImages(Name);
				task.Wait();

				return task.Result;
			}, TIMEOUT);
			_Kinks = new ExpiringLazy<Dictionary<Info.KinkInfo, Info.KinkChoice>>(() =>
			{
				var task = Client.GetKinks(Name);
				task.Wait();

				return task.Result;
			}, TIMEOUT);
		}
	}

}
