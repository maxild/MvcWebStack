using System;
using Maxfire.Core;
using Maxfire.Core.Extensions;
using Maxfire.Core.Reflection;

namespace Maxfire.Skat
{
	public class ModregningGetterAccessorPair
	{
		public ModregningGetterAccessorPair(Getter<Skatter, decimal> skatteGetter, Accessor<Skatter, decimal> modregningAccessor)
		{
			SkatteGetter = skatteGetter;
			ModregningAccessor = modregningAccessor;
		}

		/// <summary>
		/// Getter til at læse skattebeløbet.
		/// </summary>
		public Getter<Skatter, decimal> SkatteGetter { get; private set; }
		
		/// <summary>
		/// Getter og Setter (Accessor i min terminologi) til at læse og skrive til modregningsbeløbet.
		/// </summary>
		public Accessor<Skatter, decimal> ModregningAccessor { get; private set; }
	}

	public class SkatteModregner
	{
		private readonly ModregningGetterAccessorPair[] _modregningGetterAccessorPairs;

		public SkatteModregner(params ModregningGetterAccessorPair[] modregningGetterAccessorPairs)
		{
			modregningGetterAccessorPairs.ThrowIfNull("modregningGetterSetterPairs");
			if (modregningGetterAccessorPairs.Length == 0)
			{
				throw new ArgumentException(string.Format("At least one {0} must be given.", typeof(ModregningGetterAccessorPair).Name));
			}
			_modregningGetterAccessorPairs = modregningGetterAccessorPairs;
		}

		public Getter<Skatter, decimal> FirstSkatteGetter()
		{
			return _modregningGetterAccessorPairs[0].SkatteGetter;
		}

		public Accessor<Skatter, decimal> FirstModregningAccessor()
		{
			return _modregningGetterAccessorPairs[0].ModregningAccessor;
		}

		public ModregnResult Modregn(Skatter skatter, decimal skattevaerdi)
		{
			if (skattevaerdi == 0)
			{
				return new ModregnResult(skatter, skattevaerdi);
			}

			int sign = Operator<decimal>.Sign(skattevaerdi);
			skattevaerdi = Math.Abs(skattevaerdi);

			Skatter modregnedeSkatter = skatter.Clone();

			for (int i = 0; i < _modregningGetterAccessorPairs.Length && skattevaerdi > 0; i++)
			{
				var getter = _modregningGetterAccessorPairs[i].SkatteGetter;
				var accessor = _modregningGetterAccessorPairs[i].ModregningAccessor;
				decimal skat = getter.GetValue(modregnedeSkatter);
				decimal modregning = accessor.GetValue(modregnedeSkatter);
				decimal modregningAfSkattevaerdi = Math.Min(skat, skattevaerdi);
				accessor.SetValue(modregnedeSkatter, modregning + modregningAfSkattevaerdi);
				skattevaerdi -= modregningAfSkattevaerdi;
			}
			
			return new ModregnResult(modregnedeSkatter, sign * skattevaerdi);
		}
	}
}