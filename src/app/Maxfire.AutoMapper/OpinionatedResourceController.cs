using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using Maxfire.Core;
using Maxfire.Web.Mvc;

namespace Maxfire.AutoMapper.Web.Mvc
{
	public abstract class OpinionatedResourceControllerWithoutNewMethod<TController, TInputModel, TViewModel, TShowModel, TEditModel, TModel, TId> 
		: OpinionatedRestfulController<TInputModel, TEditModel, TModel, TId>
		where TController : OpinionatedResourceControllerWithoutNewMethod<TController, TInputModel, TViewModel, TShowModel, TEditModel, TModel, TId>
		where TInputModel : class, IEntityViewModel<TId>, new()
		where TModel : class, IEntity<TId>
	{
		protected OpinionatedResourceControllerWithoutNewMethod(IRepository<TModel, TId> repository, IModelUnflattener<TInputModel, TModel> modelUnflattener)
			: base(modelUnflattener)
		{
			Repository = repository;
		}

		protected IRepository<TModel, TId> Repository { get; private set; }

		// GET /models
		public virtual ActionResult Index()
		{
			var modelList = Repository.GetAll();
			object viewModelList = Mapper.Map<IEnumerable<TModel>, IEnumerable<TViewModel>>(modelList);
			return View(viewModelList);
		}

		// GET /models/{id}
		public virtual ActionResult Show(TId id)
		{
			var model = Repository.GetById(id);
			if (model == null)
			{
				FlashNotice(String.Format("Entitet med id '{0}' findes ikke.", id));
				return this.RedirectToAction<TController>(x => x.Index());
			}
			var showModel = Mapper.Map<TModel, TShowModel>(model);
			return View(showModel);
		}

		// GET /models/{id}/edit
		public virtual ActionResult Edit(TId id)
		{
			var model = Repository.GetById(id);
			if (model == null)
			{
				FlashNotice("Entitet findes ikke.");
				return this.RedirectToAction<TController>(x => x.Index());
			}
			var editModel = getEditModelFor(model);
			return EditViewFor(editModel);
		}

		// POST /models
		public override ActionResult Create(TInputModel input)
		{
			return ProcessInput(input, () => ModelUnflattener.MapInputToModel(input),
			                    () => GetEditViewFor(input),
			                    () => GetEditViewFor(input),
			                    model =>
			                    	{
			                    		Repository.Save(model);
			                    		FlashNotice("Entitet er blevet gemt.");
			                    		return this.RedirectToAction<TController>(x => x.Show(model.Id));
			                    	});
		}

		// PUT /models/{id}
		public override ActionResult Update(TInputModel input)
		{
			return ProcessInput(input, () => ModelUnflattener.MapInputToModel(input),
			                    () => GetEditViewFor(input),
			                    () => GetEditViewFor(input),
			                    model =>
			                    	{
			                    		Repository.Save(model);
			                    		FlashNotice("Entitet er blevet opdateret.");
			                    		return this.RedirectToAction<TController>(x => x.Index());
			                    	});
		}

		// DELETE /models/{id}
		public virtual ActionResult Destroy(TId id)
		{
			TModel model = Repository.GetProxyById(id);
			Repository.Delete(model);
			FlashNotice("Entitet er blevet slettet.");
			return this.RedirectToAction<TController>(x => x.Index());
		}

		protected abstract TEditModel GetEditModelFor(TInputModel input);

		protected ActionResult GetEditViewFor(TInputModel input)
		{
			var editModel = GetEditModelFor(input);
			return EditViewFor(editModel);
		}

		private TEditModel getEditModelFor(TModel model)
		{
			var input = Mapper.Map<TModel, TInputModel>(model);
			return GetEditModelFor(input);
		}
	}

	public abstract class OpinionatedResourceController<TController, TInputModel, TViewModel, TShowModel, TEditModel, TModel, TId> 
		: OpinionatedResourceControllerWithoutNewMethod<TController, TInputModel, TViewModel, TShowModel, TEditModel, TModel, TId>
		where TController : OpinionatedResourceController<TController, TInputModel, TViewModel, TShowModel, TEditModel, TModel, TId>
		where TInputModel : class, IEntityViewModel<TId>, new()
		where TModel : class, IEntity<TId>
	{
		// GET /models/new
		protected OpinionatedResourceController(IRepository<TModel, TId> repository, IModelUnflattener<TInputModel, TModel> modelUnflattener) 
			: base(repository, modelUnflattener)
		{
		}

		public virtual ActionResult New()
		{
			return GetEditViewFor(new TInputModel());
		}
	}
}