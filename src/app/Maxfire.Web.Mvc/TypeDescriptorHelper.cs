using System;
using System.ComponentModel;

namespace Maxfire.Web.Mvc
{
	public static class TypeDescriptorHelper
	{
		public static ICustomTypeDescriptor GetTypeDescriptor(Type type)
		{
			return TypeDescriptor.GetProvider(type).GetTypeDescriptor(type);
		}
	}
}