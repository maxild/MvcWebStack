using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using Maxfire.Core;

namespace Maxfire.Web.Mvc
{
	[SuppressMessage("ReSharper", "UnusedTypeParameter")]
	public abstract class OpinionatedSubordinateResourceController<TInputModel, TViewModel, TShowModel, TEditModel, TParentModel, TModel, TId>
		: OpinionatedRestfulController<TInputModel, TEditModel, TModel, TId>
		where TInputModel : class, IEntityViewModel<TId>, new()
		where TModel : class, IEntity<TId>
	{
		protected OpinionatedSubordinateResourceController(IRepository<TParentModel, TId> repository, IModelUnflattener<TInputModel, TModel> modelUnflattener)
			: base(modelUnflattener)
		{
			Repository = repository;
		}

		protected IRepository<TParentModel, TId> Repository { get; }

		// GET /parents/{parentId}/models
		public abstract ActionResult Index(TId parentId);

		// GET /parents/{parentId}/models/{id}
		public virtual ActionResult Show(TId parentId, TId id)
		{
			// todo
			return null;
		}

		// GET /parents/{parentId}/models/new
		public abstract ActionResult New(TId parentId);

		public virtual ActionResult Edit(TId parentId, TId id)
		{
			// todo
			return null;
		}

		// DELETE /parents/{parentId}/models/{id}
		public virtual ActionResult Destroy(TId parentId, TId id)
		{
			// todo
			return null;
		}

		// Todo: Need to grab the parent here, because it is part of the child edit model
		protected abstract TEditModel GetEditModelFor(TInputModel input);

		protected ActionResult GetEditViewFor(TInputModel input)
		{
			var editModel = GetEditModelFor(input);
			return EditViewFor(editModel);
		}
	}
}
