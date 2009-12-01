using System;
using System.Data;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.UserTypes;

namespace Maxfire.NHibernate
{
	public abstract class ImmutableCompositeUserType<T> : ICompositeUserType
	{
		public abstract object GetPropertyValue(object component, int property);

		public virtual void SetPropertyValue(object component, int property, object value)
		{
			throw new Exception(string.Format("{0} is an immutable type.", typeof(T).FullName));
		}

		public new virtual bool Equals(object x, object y)
		{
			return Object.Equals(x, y);
		}

		public virtual int GetHashCode(object x)
		{
			return x.GetHashCode();
		}

		public abstract object NullSafeGet(IDataReader dr, string[] names, ISessionImplementor session, object owner);

		public abstract void NullSafeSet(IDbCommand cmd, object value, int index, ISessionImplementor session);

		public virtual object DeepCopy(object value)
		{
			return value;
		}

		public virtual object Disassemble(object value, ISessionImplementor session)
		{
			return value;
		}

		public virtual object Assemble(object cached, ISessionImplementor session, object owner)
		{
			return cached;
		}

		public virtual object Replace(object original, object target, ISessionImplementor session, object owner)
		{
			return original;
		}

		public abstract string[] PropertyNames { get; }

		public abstract IType[] PropertyTypes { get; }

		public Type ReturnedClass
		{
			get { return typeof(T); }
		}

		public bool IsMutable
		{
			get { return false; }
		}
	}
}