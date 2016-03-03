using System.Web;
using System.Xml;

namespace Maxfire.Web.Mvc.UnitTests.Html5.AssertionExtensions
{
	public static class HtmlStringAssertionExtensions
	{
		public static ElementVerifier VerifyThatElement(this IHtmlString fragment)
		{
			string xhtml = fragment.ToHtmlString();
			return new ElementVerifier(xhtml, xhtml.ToXmlDoc());
		}

		public static ElementListVerifier VerifyThatElementList(this IHtmlString fragment)
		{
			string xhtml = fragment.ToHtmlString();
			return new ElementListVerifier(xhtml, xhtml.ToXmlDoc(true));
		}

		private static XmlDocument ToXmlDoc(this string xhtml, bool wrap = false)
		{
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