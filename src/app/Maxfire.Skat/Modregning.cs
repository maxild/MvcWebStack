using System;
using System.Linq.Expressions;
using Maxfire.Core.Reflection;

namespace Maxfire.Skat
{
	public static class Modregning
	{
		public static Accessor<Skatter, decimal > Af(Expression<Func<Skatter, decimal>> expression)
		{
			return IntrospectionOf<Skatter>.GetAccessorFor(expression);
		}
	}
}