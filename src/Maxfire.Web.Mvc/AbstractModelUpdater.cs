using Maxfire.Core.Extensions;

namespace Maxfire.Web.Mvc
{
	public abstract class AbstractModelUpdater<TInputModel, TModel> : IModelUpdater<TInputModel, TModel>
		where TInputModel : class
		where TModel : class
	{
		protected AbstractModelUpdater() : this(new MappingConventions())
		{
		}

		protected AbstractModelUpdater(MappingConventions conventions)
		{
			Conventions = conventions;
		}

		protected MappingConventions Conventions { get; set; }

		public virtual TModel MapToTransient(TInputModel input)
		{
			input.ThrowIfNull("input");

			return MapToModel(input, null);
		}

		public virtual void MapToPersistent(TInputModel input, TModel model)
		{
			input.ThrowIfNull("input");
			model.ThrowIfNull("model");

			MapToModel(input, model);
		}

		public virtual ValidationResult Validate(TInputModel input)
		{
			input.ThrowIfNull("input");

			var validationResult = new ValidationResult();

			MapToModelCore(validationResult, input, null);

			return validationResult;
		}

		private TModel MapToModel(TInputModel input, TModel model)
		{
			var validationResult = new ValidationResult();

			model = MapToModelCore(validationResult, input, model);

			if (!validationResult.IsValid)
			{
				throw new ValidationException(validationResult);
			}

			return model;
		}

		protected abstract TModel MapToModelCore(ValidationResult validationResult, TInputModel input, TModel model);
	}
}
