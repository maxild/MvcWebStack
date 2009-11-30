using System.Collections.Generic;
using System.Web.Mvc;
using Maxfire.Web.Mvc.FluentHtml.Behaviors;

namespace Maxfire.Web.Mvc.FluentHtml
{
	public interface IViewModelContainer<T> where T : class
	{
		T ViewModel { get; }
		ModelStateDictionary ViewModelState { get; }
		IEnumerable<IBehaviorMarker> Behaviors { get; }
		string HtmlNamePrefix { get; set; }
	}
}