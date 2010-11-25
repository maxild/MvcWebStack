using System.Text;

namespace Maxfire.Web.Mvc.Html5.Elements
{
	/// <summary>
	/// Base class for a list of elements with the same tagName where only text/value attributes can vary.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class ElementList<T> : Fragment<T> where T : ElementList<T>
	{
		protected ElementList(string tagName) : base(tagName)
		{
		}

		protected abstract bool PreRender();

		public override string ToHtmlString()
		{
			var stringBuilder = new StringBuilder();
			while (PreRender())
			{
				stringBuilder.Append(base.ToHtmlString());
			}
			return stringBuilder.ToString();
		}
	}
}