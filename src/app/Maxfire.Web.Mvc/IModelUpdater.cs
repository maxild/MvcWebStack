namespace Maxfire.Web.Mvc
{
	public interface IModelUpdater<TInputModel, TModel>
	{
		TModel MapToTransient(TInputModel input);
		void MapToPersistent(TInputModel input, TModel model);
		ValidationResult Validate(TInputModel input);
	}
}