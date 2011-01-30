namespace Maxfire.Web.Mvc.Html5.Elements
{
	public abstract class OptionsFormElement<T> : OptionsFormFragment<T> where T : OptionsFormElement<T>
	{
		protected OptionsFormElement(string elementName, string name, IModelMetadataAccessor accessor)
			: base(elementName, name, accessor)
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