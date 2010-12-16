using Maxfire.Web.Mvc;
using Maxfire.Web.Mvc.FluentHtml.Elements;

namespace Maxfire.Spark.Web.Mvc
{
	public static class ResourceViewExtensions
	{
		// Todo: Overvej at skrive hele FluentHtml extension methods om til at benytte resource view, og benyt kun FluentHtml som form helpers
		public static string Method<TEditModel, TInputModel, TId>(
			this OpinionatedResourceSparkView<TEditModel, TInputModel, TId> resourceView)
			where TEditModel : EditModelFor<TInputModel>
			where TInputModel : class, IEntityViewModel<TId>
		{
			string html = string.Empty;
			if (!resourceView.ViewModel.Input.IsTransient)
			{
				html = new Method().ToString();
			}
			return html;
		}
	}
}