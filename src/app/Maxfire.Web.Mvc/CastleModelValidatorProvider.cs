using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Castle.Components.Validator;

namespace Maxfire.Web.Mvc
{
	public class CastleModelValidatorProvider : OpinionatedValidatorProvider
	{
		private readonly IValidatorRunner _validatorRunner;

		public CastleModelValidatorProvider(IValidatorRunner validatorRunner)
		{
			_validatorRunner = validatorRunner;
		}

		protected override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context, IEnumerable<Attribute> attributes)
		{
			if (metadata == null)
			{
				throw new ArgumentNullException("metadata");
			}
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}

			yield return new CastleModelValidator(metadata, context, _validatorRunner);
		}
	}
}