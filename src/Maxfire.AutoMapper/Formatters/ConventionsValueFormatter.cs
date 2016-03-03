using Maxfire.Web.Mvc;

namespace Maxfire.AutoMapper.Web.Mvc.Formatters
{
	public abstract class ConventionsValueFormatter<T> : ValueFormatter<T>
	{
		protected ConventionsValueFormatter() : this(new MappingConventions())
		{
		}

		protected ConventionsValueFormatter(MappingConventions conventions)
		{
			Conventions = conventions;
		}

		protected MappingConventions Conventions { get; set; }
	}
}