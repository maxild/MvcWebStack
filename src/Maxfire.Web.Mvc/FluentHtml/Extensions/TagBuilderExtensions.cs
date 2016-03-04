using System;
using System.Linq;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc.FluentHtml.Extensions
{
	public static class TagBuilderExtensions
	{
		public static void AddAttribute(this TagBuilder tagBuilder, string name, string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				tagBuilder.MergeAttribute(name, value, true);
			}
		}

		public static void AddCssClassOnlyOnce(this TagBuilder tagBuilder, string @class)
		{
			if (!tagBuilder.CssClasses().Contains(@class))
			{
				tagBuilder.AddCssClass(@class);
			}
		}

		public static bool HasCssClass(this TagBuilder tagBuilder, string @class)
		{
			return tagBuilder.CssClasses().Contains(@class);
		}

		public static bool HasAttribute(this TagBuilder tagBuilder, string name)
		{
			return tagBuilder.Attributes.ContainsKey(name);
		}

		private static string[] CssClasses(this TagBuilder tagBuilder)
		{
			return tagBuilder.CssClass().Split(new [] {' '}, StringSplitOptions.RemoveEmptyEntries);
		}

		private static string CssClass(this TagBuilder tagBuilder)
		{
			string currentValue;
			return tagBuilder.Attributes.TryGetValue("class", out currentValue) ? currentValue : string.Empty;
		}
	}
}
