using System;
using System.Linq;
using Maxfire.Core.Reflection;
using Maxfire.Web.Mvc.FluentHtml.Elements;

namespace Maxfire.Web.Mvc.FluentHtml.Behaviors
{
	public class LabelMemberBehavior : IMemberBehavior
	{
		// Not all elements should have a label (e.g. Hidden, SubmitButton)
		private static readonly Type[] WHITE_LIST = {
		                                            		// Add more input input elements if neeeded
		                                            		typeof(TextBox),
		                                            		typeof(Select)
		                                            	};

		public void Execute(IMemberElement element)
		{
			if (ShouldRenderLabel(element))
			{
			    if (element is ILabeledElement e)
				{
					e.LabelBeforeText = element.ForMember.Member.GetDisplayName() + ": ";
					e.LabelClass = CssClass;
				}
			}
		}

		/// <summary>
		/// The CSS class on the label
		/// </summary>
		public string CssClass { get; set; }

		private static bool ShouldRenderLabel(IElement element)
		{
			bool renderLabel = WHITE_LIST.Any(type => type == element.GetType());
			return renderLabel;
		}
	}
}
