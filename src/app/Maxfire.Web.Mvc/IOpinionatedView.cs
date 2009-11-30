using Maxfire.Web.Mvc.FluentHtml;

namespace Maxfire.Web.Mvc
{
	public interface IOpinionatedView<TViewModel> : IViewModelContainer<TViewModel>, IUrlResponseWriter
		where TViewModel : class
	{
	}
}