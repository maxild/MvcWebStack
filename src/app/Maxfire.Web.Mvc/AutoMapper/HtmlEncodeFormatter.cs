using System.Web;
using AutoMapper;
using Maxfire.Core.Extensions;

namespace Maxfire.Web.Mvc.AutoMapper
{
	public class HtmlEncodeFormatter : IValueFormatter
	{
		public string FormatValue(ResolutionContext context)
		{
			return HttpUtility.HtmlEncode(context.SourceValue.ToNullSafeString());
		}
	}
}