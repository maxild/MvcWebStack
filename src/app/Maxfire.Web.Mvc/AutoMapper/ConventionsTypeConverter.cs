using AutoMapper;

namespace Maxfire.Web.Mvc.AutoMapper
{
	public abstract class ConventionsTypeConverter<TSource, TDest> : TypeConverter<TSource, TDest>, IConventionsTypeConverter<TSource, TDest>
	{
		protected ConventionsTypeConverter() : this(new MappingConventions())
		{
		}

		protected ConventionsTypeConverter(MappingConventions conventions)
		{
			Conventions = conventions;
		}

		protected MappingConventions Conventions { get; set; }

		public TDest Convert(TSource source)
		{
			return ConvertCore(source);
		}
	}
}