using System.Web;
using System.Xml;

namespace Maxfire.Web.Mvc.UnitTests.Html5.AssertionExtensions
{
	public static class HtmlStringAssertionExtensions
	{
		public static ElementVerifier VerifyThatElement(this IHtmlString fragment)
		{
			return new ElementVerifier(fragment.ToXmlDoc());
		}

		public static ElementListVerifier VerifyThatElementList(this IHtmlString fragment)
		{
			return new ElementListVerifier(fragment.ToXmlDoc(true));
		}

		private static XmlDocument ToXmlDoc(this IHtmlString fragment, bool wrap = false)
		{
			string xhtml = fragment.ToHtmlString();
			if (wrap)
			{
				xhtml = string.Concat("<wrap>", xhtml, "</wrap>");
			}
			var document = new XmlDocument();
			document.LoadXml(xhtml);
			return document;
		}
	}
}