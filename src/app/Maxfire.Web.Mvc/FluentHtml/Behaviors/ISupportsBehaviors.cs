using System.Collections.Generic;

namespace Maxfire.Web.Mvc.FluentHtml.Behaviors
{
	public interface ISupportsBehaviors
	{
		void ApplyBehaviors(IEnumerable<IBehaviorMarker> behaviors);
	}
}