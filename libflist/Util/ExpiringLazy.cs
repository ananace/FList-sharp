using System;
using System.Threading;

namespace libflist.Util
{

	sealed class ExpiringLazy<T>
	{
		readonly Func<T> _Factory;
		readonly TimeSpan _Lifetime;

		object _Lock = new object();
		T _Value = default(T);
		bool _HasValue = false;
		DateTime _UpdateDate;

		LazyThreadSafetyMode ThreadSafetyMode { get; set; }

		public ExpiringLazy(Func<T> Factory, TimeSpan Lifetime, LazyThreadSafetyMode Mode = LazyThreadSafetyMode.PublicationOnly)
		{
			_Factory = Factory;
			_Lifetime = Lifetime;

			ThreadSafetyMode = Mode;
		}

		public void Reset()
		{
			_HasValue = false;
		}

		public T Value
		{
			get
			{
				if (!_HasValue || (_Lifetime != Timeout.InfiniteTimeSpan && DateTime.Now > _UpdateDate + _Lifetime))
				{
					_HasValue = false;

					if (ThreadSafetyMode == LazyThreadSafetyMode.ExecutionAndPublication)
					{
						lock (_Lock)
						{
							if (_HasValue)
								return _Value;

							_UpdateDate = DateTime.Now;
							_Value = _Factory();
							_HasValue = true;

							return _Value;
						}
					}
					else if (ThreadSafetyMode == LazyThreadSafetyMode.PublicationOnly)
					{
						var newValue = _Factory();

						lock (_Lock)
						{
							if (_HasValue)
								return _Value;

							_UpdateDate = DateTime.Now;
							_Value = newValue;
							_HasValue = true;

							return _Value;
						}
					}
					else
					{
						_UpdateDate = DateTime.Now;
						_Value = _Factory();
						_HasValue = true;
					}
					
				}

				return _Value;
			}
		}

		public T UnderlyingValue
		{
			get { return _Value; }
			set { _Value = value; }
		}
	}

}
