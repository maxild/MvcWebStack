using System.Collections.Generic;

namespace Maxfire.Core
{
	public interface IRepository<TEntityClass, TId>
	{
		IEnumerable<TEntityClass> GetAll();
		TEntityClass GetById(TId id);
		TEntityClass GetProxyById(TId id);
		void Save(TEntityClass entity);
		void Delete(TEntityClass entity);
	}
}