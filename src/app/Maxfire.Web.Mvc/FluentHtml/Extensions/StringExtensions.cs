using System.Text.RegularExpressions;

namespace Maxfire.Web.Mvc.FluentHtml.Extensions
{
	public static class StringExtensions
	{
		public static string FormatAsHtmlId(this string name)
		{
			return string.IsNullOrEmpty(name)
			       	? string.Empty
			       	: Regex.Replace(name, "[^a-zA-Z0-9-:]", "_");
		}

		public static string FormatAsHtmlName(this string name)
		{
			return string.IsNullOrEmpty(name)
			       	? string.Empty
			       	: Regex.Replace(name, "[^a-zA-Z0-9.-:]", "_");
		}
	}
}