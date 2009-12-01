using System;
using System.Data;
using Maxfire.Core;
using NHibernate;
using NHibernate.SqlTypes;

namespace Maxfire.NHibernate
{
	public abstract class StrategyUserType<T> : ImmutableUserType<T>
	{
		public override object NullSafeGet(IDataReader rs, string[] names, object owner)
		{
			object obj = NHibernateUtil.String.NullSafeGet(rs, names[0]);
			if (obj == null) return null;
			var type = (string)obj;
			return CreateInstance(type);
		}

		public override void NullSafeSet(IDbCommand cmd, object value, int index)
		{
			NHibernateUtil.String.NullSafeSet(cmd, value.GetType().Name, index);
		}

		public override SqlType[] SqlTypes
		{
			get { return new[] { SqlTypeFactory.GetString(100) }; }
		}

		protected virtual T CreateInstance(string type)
		{
			var typeName = TypePath.Combine(TypePath.GetNamespacePath(typeof(T).FullName), type);
			var assemblyQualifiedTypeName = typeName + ", " + typeof(T).Assembly.FullName;
			var t = Type.GetType(assemblyQualifiedTypeName, false, true);
			if (t == null)
			{
				throw new ArgumentException(String.Format("Unknown type '{0}'.", typeName));
			}
			return (T)Activator.CreateInstance(t);
		}
	}
}