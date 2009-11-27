using System;
using System.Collections;
using Castle.Components.Validator;
using Maxfire.Core.Extensions;

namespace Maxfire.Web.Mvc.Validators
{
	public class LabeledValidateCollectionNotEmptyAttribute : BaseValidationAttribute
	{
		private const string DEFAULT_ERROR_MESSAGE = "Feltet '{0}' skal indeholde mindst en v�rdi.";

		public LabeledValidateCollectionNotEmptyAttribute() 
			: base(() => new LabeledCollectionNotEmptyValidator(), DEFAULT_ERROR_MESSAGE)
		{
		}

		public class LabeledCollectionNotEmptyValidator : BaseValidator
		{
			public override bool IsValid(object instance, object fieldValue)
			{
				var collection = fieldValue as IEnumerable;
				if (collection == null)
				{
					return false;
				}

				int count = 0;
				collection.Each(item => count++);

				return count != 0; 
			}

			public override bool IsValidCore(string fieldValue)
			{
				throw new NotImplementedException("This method is not called, because of the IsValid override.");
			}

			public override bool SupportsBrowserValidation
			{
				get { return false; }
			}
		}
	}
}