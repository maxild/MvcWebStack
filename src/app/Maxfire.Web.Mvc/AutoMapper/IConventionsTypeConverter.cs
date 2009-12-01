using AutoMapper;

namespace Maxfire.Web.Mvc.AutoMapper
{
	public interface IConventionsTypeConverter<TSource, TDest> : ITypeConverter<TSource, TDest>
	{
		TDest Convert(TSource source);
	}
}