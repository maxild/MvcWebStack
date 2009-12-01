using AutoMapper;

namespace Maxfire.Web.Mvc.AutoMapper
{
	public abstract class ConventionsValueResolver<TSource, TDestination> : ValueResolver<TSource, TDestination>
	{
		protected ConventionsValueResolver() : this(new MappingConventions())
		{
		}

		protected ConventionsValueResolver(MappingConventions conventions)
		{
			Conventions = conventions;
		}

		public MappingConventions Conventions { get; private set; }
	}
}