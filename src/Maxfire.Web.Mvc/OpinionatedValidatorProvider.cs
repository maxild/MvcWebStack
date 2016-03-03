using System;
using System.ComponentModel;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc
{
	public abstract class OpinionatedValidatorProvider : AssociatedValidatorProvider
	{
		protected override ICustomTypeDescriptor GetTypeDescriptor(Type type)
		{
			return TypeDescriptorHelper.GetTypeDescriptor(type);
		} 
	}
}