using AutoMapper;

namespace Maxfire.AutoMapper.Web.Mvc
{
	public interface IConventionsTypeConverter<TSource, TDest> : ITypeConverter<TSource, TDest>
	{
		TDest Convert(TSource source);
	}
}