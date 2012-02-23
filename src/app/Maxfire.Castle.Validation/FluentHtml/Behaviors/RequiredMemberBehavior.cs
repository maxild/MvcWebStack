using Maxfire.Castle.Validation.Validators;
using Maxfire.Core.Reflection;
using Maxfire.Web.Mvc.FluentHtml;
using Maxfire.Web.Mvc.FluentHtml.Behaviors;

namespace Maxfire.Castle.Validation.FluentHtml.Behaviors
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