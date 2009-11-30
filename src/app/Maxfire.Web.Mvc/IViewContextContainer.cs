using System.Web.Mvc;
using Maxfire.Web.Mvc.FluentHtml;

namespace Maxfire.Web.Mvc
{
	public interface IViewContextContainer
	{
		ViewContext ViewContext { get; }
	}

	public interface IViewModelAndContextContainer<TViewModel> : IViewModelContainer<TViewModel>, IViewContextContainer
		where TViewModel : class
	{
	}
}