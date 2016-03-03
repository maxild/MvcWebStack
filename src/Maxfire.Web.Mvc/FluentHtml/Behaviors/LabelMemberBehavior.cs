using System;
using System.Linq;
using Maxfire.Core.Reflection;
using Maxfire.Web.Mvc.FluentHtml.Elements;

namespace Maxfire.Web.Mvc.FluentHtml.Behaviors
{
	public class LabelMemberBehavior : IMemberBehavior
	{
		// Not all elements should have a label (e.g. Hidden, SubmitButton)
		private static readonly Type[] _whiteList = new []
		                                            	{
		                                            		// Add more input input elements if neeeded
		                                            		typeof(TextBox),
		                                            		typeof(Select)
		                                            	};

		public void Execute(IMemberElement element)
		{
			if (shouldRenderLabel(element))
			{
				ILabeledElement e = element as ILabeledElement;
				if (e != null)
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

		private static bool shouldRenderLabel(IElement element)
		{
			bool renderLabel = _whiteList.Any(type => type == element.GetType());
			return renderLabel;
		}
	}
}