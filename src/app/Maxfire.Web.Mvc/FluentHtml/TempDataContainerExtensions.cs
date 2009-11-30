using Maxfire.Core.Extensions;
using Maxfire.Web.Mvc.FluentHtml.Elements;
using Maxfire.Web.Mvc.FluentHtml.Html;

namespace Maxfire.Web.Mvc.FluentHtml
{
	public static class TempDataContainerExtensions
	{
		public static Literal ShowNotice(this ITempDataContainer view)
		{
			string notice = view.TempData[HtmlCssClass.FLASH_NOTICE].ToNullSafeString();
			if (notice.IsEmpty())
			{
				return null;
			}
			return new Literal().Class(HtmlCssClass.FLASH_NOTICE).Value(notice);
		}
	}
}