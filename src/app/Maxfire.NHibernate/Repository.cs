using System;
using System.Collections.Generic;
using Maxfire.Core;
using NHibernate;
using NHibernate.Criterion;

namespace Maxfire.NHibernate
{
	public class Repository<TEntityClass, TId> : IRepository<TEntityClass, TId>
	{
		private readonly ISession _session;

		public Repository(ISession session)
		{
			if (session == null)
				throw new ArgumentNullException("session");

			_session = session;
		}
		
		public IEnumerable<TEntityClass> GetAll()
		{
			var criteria = _session.CreateCriteria(typeof(TEntityClass));
			criteria.SetCacheable(true);
			var list = criteria.List<TEntityClass>();
			return list;
		}

		public TEntityClass GetById(TId id)
		{
			var entity = _session.Get<TEntityClass>(id);
			return entity;
		}

		public TEntityClass GetProxyById(TId id)
		{
			var entity = _session.Load<TEntityClass>(id);
			return entity;
		}

		public void Save(TEntityClass entity)
		{
			using (ITransaction transaction = _session.BeginTransaction())
			{
				_session.SaveOrUpdate(entity);
				transaction.Commit();
			}
		}

		public void Delete(TEntityClass entity)
		{
			using (ITransaction transaction = _session.BeginTransaction())
			{
				_session.Delete(entity);
				transaction.Commit();
			}
		}

		public IEnumerable<TEntityClass> FindAll(CriterionSet criterionSet)
		{
			var persistentObjects = new List<TEntityClass>();

			var criteria = _session.CreateCriteria(typeof(TEntityClass));
			criteria.SetCacheable(true);

			foreach (var criterion in criterionSet.GetCriteria())
			{
				ICriterion expression;

				if (criterion.Operator == ComparisonOperator.GreaterThan)
				{
					expression = Restrictions.Gt(criterion.Attribute, criterion.Value);
				}
				else if (criterion.Operator == ComparisonOperator.LessThan)
				{
					expression = Restrictions.Lt(criterion.Attribute, criterion.Value);
				}
				else if (criterion.Operator == ComparisonOperator.NotEqual)
				{
					if (criterion.Value == null)
					{
						expression = Restrictions.IsNotNull(criterion.Attribute);
					}
					else
					{
						expression = Restrictions.Not(Restrictions.Eq(criterion.Attribute, criterion.Value));
					}
				}
				else
				{
					if (criterion.Value == null)
					{
						expression = Restrictions.IsNull(criterion.Attribute);
					}
					else
					{
						expression = Restrictions.Eq(criterion.Attribute, criterion.Value);
					}
				}

				criteria.Add(expression);
			}

			if (criterionSet.OrderBy != null)
			{
				if (criterionSet.SortOrder == SortOrder.Descending)
				{
					criteria.AddOrder(Order.Desc(criterionSet.OrderBy));
				}
				else
				{
					criteria.AddOrder(Order.Asc(criterionSet.OrderBy));
				}
			}

			var list = criteria.List();
			foreach (TEntityClass entity in list)
			{
				persistentObjects.Add(entity);
			}

			return persistentObjects;
		}
	}
}