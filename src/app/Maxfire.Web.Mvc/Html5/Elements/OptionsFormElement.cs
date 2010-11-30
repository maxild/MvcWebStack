using System.Web.Mvc;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	public abstract class OptionsFormElement<T> : OptionsFormFragment<T> where T : OptionsFormElement<T>
	{
		protected OptionsFormElement(string tagName, string name, IModelMetadataAccessor accessor)
			: base(tagName, name, accessor)
		{
		}

		protected override string ToTagString()
		{
			InnerHtml(RenderOptions());
			return base.ToTagString();
		}

		protected abstract string RenderOptions();
	}
}