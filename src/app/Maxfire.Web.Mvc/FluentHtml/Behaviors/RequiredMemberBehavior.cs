using Maxfire.Core.Reflection;
using Maxfire.Web.Mvc.Validators;

namespace Maxfire.Web.Mvc.FluentHtml.Behaviors
{
	/// <summary>
	/// Only an experiment, not production code.
	/// </summary>
	public class RequiredMemberBehavior : IMemberBehavior
	{
		public void Execute(IMemberElement element)
		{
			var attribute = element.ForMember.Member.GetCustomAttribute<LabeledValidateNonEmptyAttribute>();
			if (attribute != null)
			{
				element.AddCssClass("required");
			}
		}
	}
}