using System;

namespace Maxfire.Core
{
	[Serializable]
	public abstract class EntityEnumeration<TEnumeration> : ComparableEnumeration<TEnumeration>
		where TEnumeration : EntityEnumeration<TEnumeration>
	{
		public static TEntityEnumeration FromIdOrDefault<TEntityEnumeration>(long id) where TEntityEnumeration : EntityEnumeration<TEnumeration>
		{
			return FromValueOrDefault<TEntityEnumeration>((int)id);
		}

		public static TEntityEnumeration FromIdOrDefault<TEntityEnumeration>(int id) where TEntityEnumeration : EntityEnumeration<TEnumeration>
		{
			return FromValueOrDefault<TEntityEnumeration>(id);
		}

		public static TEntityEnumeration FromId<TEntityEnumeration>(long id) where TEntityEnumeration : EntityEnumeration<TEnumeration>
		{
			return FromValue<TEntityEnumeration>((int)id);
		}

		public static TEntityEnumeration FromId<TEntityEnumeration>(int id) where TEntityEnumeration : EntityEnumeration<TEnumeration>
		{
			return FromValue<TEntityEnumeration>(id);
		}

		protected EntityEnumeration()
		{
		}

		protected EntityEnumeration(int id, string name, string text) : base(id, name, text)
		{
		}

		public int Id
		{
			get { return Value; }
		}
	}
}