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
		internal ExpiringLazy<IReadOnlyCollection<Info.ImageInfo>> _Images;
		internal ExpiringLazy<IReadOnlyDictionary<Info.KinkInfo, Info.KinkChoice>> _Kinks;

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

		public Character() : this (null, null)
		{
		}
		public Character(IFListClient client, string name) : this(client, name, -1)
		{
		}
		public Character(IFListClient client, string name, int id)
		{
			Client = client;
			Name = name;
			ID = id;

			if (Client != null && Name != null)
			{
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
				_Images = new ExpiringLazy<IReadOnlyCollection<Info.ImageInfo>>(() =>
				{
					var task = Client.GetImages(Name);
					task.Wait();

					return task.Result;
				}, TIMEOUT);
				_Kinks = new ExpiringLazy<IReadOnlyDictionary<Info.KinkInfo, Info.KinkChoice>>(() =>
				{
					var task = Client.GetKinks(Name);
					task.Wait();

					return task.Result;
				}, TIMEOUT);
			}
			else
			{
				_Description = new ExpiringLazy<string>(() => null, TimeSpan.MaxValue);
				_ProfileInfo = new ExpiringLazy<Info.ProfileInfo>(() => null, TimeSpan.MaxValue);
				_Images = new ExpiringLazy<IReadOnlyCollection<Info.ImageInfo>>(() => null, TimeSpan.MaxValue);
				_Kinks = new ExpiringLazy<IReadOnlyDictionary<Info.KinkInfo, Info.KinkChoice>>(() => null, TimeSpan.MaxValue);
			}
		}
	}

}
