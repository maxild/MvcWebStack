using Maxfire.Web.Mvc.FluentHtml;

namespace Maxfire.Web.Mvc
{
	public interface IRestfulOpinionatedView<TViewModel, TId> : IViewModelContainer<TViewModel>, IUrlResponseWriter 
		where TViewModel : class, IEntityViewModel<TId>
	{
	}
}