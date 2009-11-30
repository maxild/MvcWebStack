using System.Collections.Generic;
using Maxfire.Web.Mvc.FluentHtml.Behaviors;
using Maxfire.Web.Mvc.FluentHtml.Elements;

namespace Maxfire.Web.Mvc.FluentHtml.Extensions
{
	public static class ElementExtensions
	{
		public static T ApplyBehavior<T>(this T element, IBehaviorMarker behavior)
			where T : Element<T>
		{
			return element.ApplyBehaviors(new[] { behavior });
		}

		public static T ApplyBehaviors<T>(this T element, IEnumerable<IBehaviorMarker> behaviors)
			where T : Element<T>
		{
			var x = element as ISupportsBehaviors;
			if (x != null)
			{
				x.ApplyBehaviors(behaviors);
			}
			return element;
		}
	}
}