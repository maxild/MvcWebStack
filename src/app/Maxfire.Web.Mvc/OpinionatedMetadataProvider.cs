using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc
{
	/// <summary>
	/// This version does not use buddy classes
	/// </summary>
	public class OpinionatedMetadataProvider : AssociatedMetadataProvider
	{
		// Note: Also take a look at the FilterAttribute helper method (I don't understand it)

		protected override ModelMetadata CreateMetadata(IEnumerable<Attribute> attributes, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
		{
			var metadata = new ModelMetadata(this, containerType, modelAccessor, modelType, propertyName);

			//var displayFormatAttribute = attributes.OfType<DisplayFormatAttribute>().FirstOrDefault();
			//if (displayFormatAttribute != null)
			//{
			//    metadata.NullDisplayText = displayFormatAttribute.NullDisplayText;
			//    metadata.DisplayFormatString = displayFormatAttribute.DataFormatString;
			//    metadata.ConvertEmptyStringToNull = displayFormatAttribute.ConvertEmptyStringToNull;

			//    if (displayFormatAttribute.ApplyFormatInEditMode)
			//    {
			//        metadata.EditFormatString = displayFormatAttribute.DataFormatString;
			//    }
			//}

			var displayNameAttribute = attributes.OfType<DisplayNameAttribute>().FirstOrDefault();
			if (displayNameAttribute != null)
			{
				metadata.DisplayName = displayNameAttribute.DisplayName;
			}

			return metadata;
		}

		protected override ICustomTypeDescriptor GetTypeDescriptor(Type type)
		{
			return TypeDescriptorHelper.GetTypeDescriptor(type);
		}
	}
}