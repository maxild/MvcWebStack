using System;
using System.Linq.Expressions;
using Maxfire.Core.Reflection;

namespace Maxfire.Skat
{
	public static class Modregning
	{
		public static ModregningGetterAccessorPairBuilder Af(Expression<Func<Skatter, decimal>> expression)
		{
			return new ModregningGetterAccessorPairBuilder(IntrospectionOf<Skatter>.GetGetterFor(expression));
		}
	}

	public class ModregningGetterAccessorPairBuilder
	{
		private readonly Getter<Skatter, decimal> _getter;

		public ModregningGetterAccessorPairBuilder(Getter<Skatter, decimal> getter)
		{
			_getter = getter;
		}

		public ModregningGetterAccessorPair Med(Expression<Func<Skatter, decimal>> expression)
		{
			return new ModregningGetterAccessorPair(_getter, IntrospectionOf<Skatter>.GetAccessorFor(expression));
		}
	}
}