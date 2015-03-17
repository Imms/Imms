using System.Collections.Generic;

namespace Funq.Abstract
{
	internal class MapEquality<TKey, TValue> : IEqualityComparer<ITrait_KeyValueMap<TKey, TValue>>
	{
		private readonly IEqualityComparer<TKey> _kEquality;
		private readonly IEqualityComparer<TValue> _vEquality;

		public MapEquality(IEqualityComparer<TValue> vEquality, IEqualityComparer<TKey> kEquality)
		{
			_vEquality = vEquality;
			_kEquality = kEquality;
		}

		public bool Equals(ITrait_KeyValueMap<TKey, TValue> x, ITrait_KeyValueMap<TKey, TValue> y)
		{
			return Equality.Map_Equate(x, y, _vEquality);
		}

		public int GetHashCode(ITrait_KeyValueMap<TKey, TValue> obj)
		{
			return Equality.Map_HashCode(obj, _kEquality, _vEquality);
		}
	}
}