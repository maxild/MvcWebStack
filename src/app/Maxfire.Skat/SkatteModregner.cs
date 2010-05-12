using System;
using Maxfire.Core.Extensions;
using Maxfire.Core.Reflection;

namespace Maxfire.Skat
{
	public class SkatteModregner
	{
		private readonly Accessor<Skatter, decimal>[] _accessors;

		public SkatteModregner(params Accessor<Skatter, decimal>[] accessors)
		{
			accessors.ThrowIfNull("accessors");
			if (accessors.Length == 0)
			{
				throw new ArgumentException("At least one accessor must be given.");
			}
			_accessors = accessors;
		}

		public Accessor<Skatter, decimal> FirstAccessor()
		{
			return _accessors[0];
		}

		/// <summary>
		/// Beregner mulige modregnede skatteværdier Modregner den angivne (absolutte) skatteværdi 
		/// </summary>
		/// <param name="skatter"></param>
		/// <param name="skattevaerdi"></param>
		/// <returns></returns>
		public ModregnResult Modregn(Skatter skatter, decimal skattevaerdi)
		{
			if (skattevaerdi <= 0)
			{
				return new ModregnResult(skatter, Skatter.Nul, 0);
			}

			var modregninger = new Skatter();

			for (int i = 0; i < _accessors.Length && skattevaerdi > 0; i++)
			{
				var accessor = _accessors[i];
				decimal skat = accessor.GetValue(skatter);
				decimal modregning = accessor.GetValue(modregninger);
				decimal modregningAfSkattevaerdi = Math.Min(skat, skattevaerdi);
				accessor.SetValue(modregninger, modregning + modregningAfSkattevaerdi);
				skattevaerdi -= modregningAfSkattevaerdi;
			}
			
			return new ModregnResult(skatter, modregninger, skattevaerdi);
		}
	}
}