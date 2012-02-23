using System.Web;
using AutoMapper;
using Maxfire.Core.Extensions;

namespace Maxfire.AutoMapper.Web.Mvc.Formatters
{
	public class HtmlEncodeFormatter : IValueFormatter
	{
		public string FormatValue(ResolutionContext context)
		{
			return HttpUtility.HtmlEncode(context.SourceValue.ToNullSafeString());
		}
	}
}