using System.Collections.Generic;
using System.Linq;
using Castle.Components.Validator;
using Maxfire.Web.Mvc.Validators.Extensions;

namespace Maxfire.Web.Mvc.Validators
{
	public abstract class BaseValidator : AbstractValidator
	{
		public override bool IsValid(object instance, object fieldValue)
		{
			var typedCollection = fieldValue as IEnumerable<string>;
			if (typedCollection != null)
			{
				return typedCollection.All(item => IsValidCore(item.ToTrimmedNullSafeString()));
			}

			if (fieldValue.IsTrimmedEmpty())
			{
				return true;
			}

			return IsValidCore(fieldValue.ToTrimmedNullSafeString());
		}

		public abstract bool IsValidCore(string fieldValue);
		
		public MappingConventions Conventions
		{
			get { return new MappingConventions(); }
		}

		public override bool SupportsBrowserValidation
		{
			get { return false; }
		}
	}
}