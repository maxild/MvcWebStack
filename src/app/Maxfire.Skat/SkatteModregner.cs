using System;
using Maxfire.Core;
using Maxfire.Core.Extensions;
using Maxfire.Core.Reflection;

namespace Maxfire.Skat
{
	public class SkatteModregner
	{
		private readonly Accessor<Skatter, decimal>[] _skatteAccessors;

		public SkatteModregner(params Accessor<Skatter, decimal>[] skatteAccessors)
		{
			skatteAccessors.ThrowIfNull("skatteAccessors");
			if (skatteAccessors.Length == 0)
			{
				throw new ArgumentException("At least one skatteAccessor must be given.");
			}
			_skatteAccessors = skatteAccessors;
		}

		public Accessor<Skatter, decimal> First()
		{
			return _skatteAccessors[0];
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

			for (int i = 0; i < _skatteAccessors.Length; i++)
			{
				var skatteAccesor = _skatteAccessors[i];
				decimal modregnetSkat = skatteAccesor.GetValue(modregnedeSkatter) - skattevaerdi;
				if (modregnetSkat >= 0)
				{
					skatteAccesor.SetValue(modregnedeSkatter, modregnetSkat);
					skattevaerdi = 0;
					break;
				}
				skatteAccesor.SetValue(modregnedeSkatter, 0);
				skattevaerdi = -modregnetSkat;
			}
			
			return new ModregnResult(modregnedeSkatter, sign * skattevaerdi);
		}
	}
}