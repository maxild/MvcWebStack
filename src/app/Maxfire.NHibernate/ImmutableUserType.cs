using System;
using System.Data;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace Maxfire.NHibernate
{
	public abstract class ImmutableUserType<T> : IUserType
	{
		public abstract SqlType[] SqlTypes { get; }

		public Type ReturnedType
		{
			get { return typeof(T); }
		}

		public new bool Equals(object x, object y)
		{
			return Object.Equals(x, y);
		}

		public int GetHashCode(object x)
		{
			return x.GetHashCode();
		}

		public bool IsMutable
		{
			get { return false; }
		}

		public abstract object NullSafeGet(IDataReader rs, string[] names, object owner);

		public abstract void NullSafeSet(IDbCommand cmd, object value, int index);

		public object DeepCopy(object value)
		{
			return value;
		}

		public object Replace(object original, object target, object owner)
		{
			return original;
		}

		public object Assemble(object cached, object owner)
		{
			return cached;
		}

		public object Disassemble(object value)
		{
			return value;
		}
	}
}