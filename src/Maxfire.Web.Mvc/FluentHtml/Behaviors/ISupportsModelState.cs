using System.Web.Mvc;

namespace Maxfire.Web.Mvc.FluentHtml.Behaviors
{
	public interface ISupportsModelState
	{
		void ApplyModelState(ModelState state);
	}
}