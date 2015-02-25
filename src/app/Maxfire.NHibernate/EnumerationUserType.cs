using System.Data;
using Maxfire.Core;
using NHibernate;
using NHibernate.SqlTypes;

namespace Maxfire.NHibernate
{
	public class EnumerationUserType<TEnumeration> : ImmutableUserType<TEnumeration>
		where TEnumeration : Enumeration
	{
		public override SqlType[] SqlTypes
		{
			get { return new[] { SqlTypeFactory.Int32 }; }
		}

		public override object NullSafeGet(IDataReader rs, string[] names, object owner)
		{
			object obj = NHibernateUtil.Int32.NullSafeGet(rs, names[0]);
			if (obj == null) return null;
			int value = (int)obj;
			var type = Enumeration.FromValue<TEnumeration>(value);
			return type;
		}

		public override void NullSafeSet(IDbCommand cmd, object value, int index)
		{
			object objectToSet = null;
			if (value != null) objectToSet = ((TEnumeration)value).Value;
			NHibernateUtil.Int32.NullSafeSet(cmd, objectToSet, index);
		}
	}
}