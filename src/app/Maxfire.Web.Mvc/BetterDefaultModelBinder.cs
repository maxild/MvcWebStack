using System.Web.Mvc;

namespace Maxfire.Web.Mvc
{
	public class BetterDefaultModelBinder : DefaultModelBinder
	{
		//protected override IModelBinder GetBinder(Type modelType)
		//{
		//	var binder = Binders.GetBinder(modelType, false);
		//	return binder ?? this;
		//}

		//protected override IEnumerable<string> GetCollectionIndexes(ModelBindingContext bindingContext)
		//{
		//	string indexKey = CreateSubPropertyName(bindingContext.ModelName, "index");
		//	ValueProviderResult vpResult = bindingContext.ValueProvider.GetValue(indexKey);

		//	if (vpResult != null)
		//	{
		//		// This is the only new line. We store the comma-separated string of arbitrary indexes 
		//		// in modelstate. This way UI helpers can reach through the CollectionIndexStore and 
		//		// the ExportModelStateToTempData and ImportModelStateFromTempData are working out of the box.
		//		// This makes validation in the framework work correctly in both re-rendering
		//		// and PRG pattern re-rendering scenarioes.
		//		bindingContext.ModelState.SetModelValue(indexKey, vpResult);

		//		string[] indexesArray = vpResult.ConvertTo(typeof(string[])) as string[];
		//		if (indexesArray != null)
		//		{
		//			return indexesArray;
		//		}
		//	}

		//	return null;
		//}
	}
}