using System;
using System.Collections.Generic;

namespace libflist
{
	public class ServerVariables : IReadOnlyDictionary<string, object>
	{
		[Flags]
		public enum UserPermission
		{
			Admin = 1,
			chat_chatop = 2,
			chat_chanop = 4,
			helpdesk_chat = 8,
			helpdesk_general = 16,
			moderation_site = 32,
			reserved = 64,
			misc_grouprequests = 128,
			misc_newsposts = 256,
			misc_changelog = 512,
			misc_featurerequests = 1024,
			dev_bugreports = 2048,
			dev_tags = 4096,
			dev_kinks = 8192,
			developer = 16384,
			tester = 32768,
			subscriptions = 65536,
			former_staff = 131072
		}

		readonly Dictionary<string, object> _OtherVars = new Dictionary<string, object>();

		public void Clear()
		{
			_OtherVars.Clear();
		}

		public void SetVariable(string name, object value)
		{
			_OtherVars[name] = value;
		}

		public int Connected { get { return (int)(_OtherVars["__connected"] ?? 0); } }

		public int ChatMax { get { return (int)(_OtherVars["chat_max"] ?? 0); } }
		public float ChatTimeout { get { return (int)(_OtherVars["msg_flood"] ?? 0); } }
		public int PrivateMax { get { return (int)(_OtherVars["priv_max"] ?? 0); } }
		public int LFRPMax { get { return (int)(_OtherVars["lfrp_max"] ?? 0); } }
		public int LFRPTimeout { get { return (int)(_OtherVars["lfrp_flood"] ?? 0); } }
		public IEnumerable<string> IconBlacklist { get { return _OtherVars["permissions"] as IEnumerable<string>; } }
		public UserPermission Permissions { get { return (UserPermission)(_OtherVars["permissions"] ?? 0); } }

		#region IReadOnlyDictionary implementation

		public bool ContainsKey(string key)
		{
			return _OtherVars.ContainsKey(key);
		}

		public bool TryGetValue(string key, out object value)
		{
			return _OtherVars.TryGetValue(key, out value);
		}

		public object this[string index] {
			get {
				return _OtherVars[index];
			}
		}

		public IEnumerable<string> Keys {
			get {
				return _OtherVars.Keys;
			}
		}

		public IEnumerable<object> Values {
			get {
				return _OtherVars.Values;
			}
		}

		#endregion

		#region IEnumerable implementation

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return _OtherVars.GetEnumerator();
		}

		#endregion

		#region IEnumerable implementation

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _OtherVars.GetEnumerator();
		}

		#endregion

		#region IReadOnlyCollection implementation

		public int Count {
			get {
				return _OtherVars.Count;
			}
		}

		#endregion
	}
}
