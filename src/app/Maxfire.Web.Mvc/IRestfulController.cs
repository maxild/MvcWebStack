using System.Web.Mvc;

namespace Maxfire.Web.Mvc
{
	public interface IRestfulController<TInputModel, TId>
		where TInputModel : class, IEntityViewModel<TId>
	{
		string BindingPrefix { get; }
		ModelStateDictionary ModelState { get; }
		ActionResult Create(TInputModel input);
		ActionResult Update(TInputModel input);
	}

	/*
	public interface IResourceController<TInputModel, TId> : IRestfulController<TInputModel, TId>
		where TInputModel : class, IEntityViewModel<TId>
	{
		ActionResult Index();
		ActionResult Show(TId id);
		//ActionResult New();
		ActionResult Edit(TId id);
		ActionResult Destroy(TId id);
	}

	public interface ISubordinateResourceController<TInputModel, TId> : IRestfulController<TInputModel, TId>
		where TInputModel : class, IEntityViewModel<TId>
	{
		ActionResult Index(TId parentId);
		ActionResult Show(TId parentId, TId id);
		//ActionResult New(TId parentId);
		ActionResult Edit(TId parentId, TId id);
		ActionResult Destroy(TId parentId, TId id);
	}
	*/
}