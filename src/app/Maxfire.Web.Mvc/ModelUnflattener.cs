using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Maxfire.Core;
using Maxfire.Web.Mvc.Validators;

namespace Maxfire.Web.Mvc
{
	[UsedImplicitly]
	public class ModelUnflattener<TInputModel, TModel> : IModelUnflattener<TInputModel, TModel>
		where TInputModel : class, IEntityViewModel<long>
		where TModel: class
	{
		private readonly IRepository<TModel, long> _repository;
		private readonly IModelUpdater<TInputModel, TModel> _modelUpdater;

		public ModelUnflattener(IRepository<TModel, long> repository, IModelUpdater<TInputModel, TModel> modelUpdater)
		{
			_repository = repository;
			_modelUpdater = modelUpdater;
		}

		public virtual IDictionary<string, string[]> ValidateInput(TInputModel input)
		{
			ValidationResult validationResult;
			
			if (!input.IsTransient && _repository.GetById(input.Id) == null)
			{
				// This can happen if the resource have been destroyed by a different request/session.
				validationResult = new ValidationResult();
				validationResult.AddErrorFor<TInputModel>(x => x.Id,
				                                               string.Format("Entitet med id '{0}' findes ikke.", input.Id));
				return validationResult.GetAllErrors();
			}
			
			validationResult = _modelUpdater.Validate(input);
			var validationErrors = validationResult.GetAllErrors();
			return validationErrors;
		}

		public virtual TModel MapInputToModel(TInputModel input)
		{
			var model = input.IsTransient ? createAndUpdate(input) : getAndUpdate(input);
			return model;
		}

		private TModel getAndUpdate(TInputModel input)
		{
			var model = _repository.GetById(input.Id);
			if (model == null)
			{
				throw new InvalidOperationException(
					string.Format("Entitet med id '{0}' findes ikke.", input.Id));
			}

			_modelUpdater.MapToPersistent(input, model);
			return model;
		}

		private TModel createAndUpdate(TInputModel input)
		{
			return _modelUpdater.MapToTransient(input);
		}
	}
}