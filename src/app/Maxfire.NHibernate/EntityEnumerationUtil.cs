using Maxfire.Core;
using Maxfire.Core.Extensions;
using NHibernate;

namespace Maxfire.NHibernate
{
	public static class EntityEnumerationUtil
	{
		public static void SaveOrUpdate<TEntityEnumeration>(ISession session)
			where TEntityEnumeration : EntityEnumeration<TEntityEnumeration>
		{
			Enumeration.GetAll<TEntityEnumeration>().Each(enumeration =>
			{
				var loadedEnumeration = session.Get<TEntityEnumeration>(enumeration.Id);
				// Note: We do not delete any values that have been removed from the Enumerations
				if (loadedEnumeration == null)
				{
					session.Save(enumeration);
				}
				else
				{
					session.Merge(enumeration);
				}
			});
		}

		public static void Save<TEntityEnumeration>(ISession session)
			where TEntityEnumeration : EntityEnumeration<TEntityEnumeration>
		{
			Enumeration.GetAll<TEntityEnumeration>().Each(enumeration => session.Save(enumeration));
		}
	}
}