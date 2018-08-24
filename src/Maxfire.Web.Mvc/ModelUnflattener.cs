using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Maxfire.Core;

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
				    $"Entitet med id '{input.Id}' findes ikke.");
				return validationResult.GetAllErrors();
			}

			validationResult = _modelUpdater.Validate(input);
			var validationErrors = validationResult.GetAllErrors();
			return validationErrors;
		}

		public virtual TModel MapInputToModel(TInputModel input)
		{
			var model = input.IsTransient ? CreateAndUpdate(input) : GetAndUpdate(input);
			return model;
		}

		private TModel GetAndUpdate(TInputModel input)
		{
			var model = _repository.GetById(input.Id);
			if (model == null)
			{
				throw new InvalidOperationException(
				    $"Entitet med id '{input.Id}' findes ikke.");
			}

			_modelUpdater.MapToPersistent(input, model);
			return model;
		}

		private TModel CreateAndUpdate(TInputModel input)
		{
			return _modelUpdater.MapToTransient(input);
		}
	}
}
