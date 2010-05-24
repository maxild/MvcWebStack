namespace Maxfire.Skat
{
	/// <summary>
	/// Resultat af enten egne modregninger eller ægtefælle modregninger
	/// </summary>
	public class BeregnModregningerResult
	{
		public BeregnModregningerResult(decimal modregningSkattepligtigIndkomst, 
			SkatterAfPersonligIndkomst modregningSkatter, decimal modregningUnderskud)
		{
			ModregningSkattepligtigIndkomst = modregningSkattepligtigIndkomst;
			ModregningSkatter = modregningSkatter;
			ModregningUnderskud = modregningUnderskud;
		}

		/// <summary>
		/// Størrelsen på den modregning, der kan rummes i den skattepligtige indkomst.
		/// </summary>
		public decimal ModregningSkattepligtigIndkomst { get; private set; }

		/// <summary>
		/// Størrelsen af de modregninger af skatteværdier, der kan rummes i skatterne.
		/// </summary>
		public SkatterAfPersonligIndkomst ModregningSkatter { get; private set; }

		/// <summary>
		/// Reduktionen i underskuddet som følge af denne modregning.
		/// </summary>
		public decimal ModregningUnderskud { get; private set; }

		public decimal GetRestunderskud(decimal underskud)
		{
			return underskud - ModregningUnderskud;
		}

		public static BeregnModregningerResult operator+ (BeregnModregningerResult lhs, BeregnModregningerResult rhs)
		{
			return new BeregnModregningerResult(
				lhs.ModregningSkattepligtigIndkomst + rhs.ModregningSkattepligtigIndkomst,
				lhs.ModregningSkatter + rhs.ModregningSkatter,
				lhs.ModregningUnderskud + rhs.ModregningUnderskud);
		}
	}
	
	public static class BeregnModregningerResultExtensions
	{
		public static ValueTuple<ModregnUnderskudResult> ToModregnResult(this ValueTuple<BeregnModregningerResult> beregnModregningerResults, 
			ValueTuple<decimal> skattepligtigeIndkomster, ValueTuple<SkatterAfPersonligIndkomst> skatter, ValueTuple<decimal> underskud)
		{
			return beregnModregningerResults.Map((result, index) =>
				new ModregnUnderskudResult(skattepligtigeIndkomster[index], result.ModregningSkattepligtigIndkomst,
					skatter[index], result.ModregningSkatter, underskud[index], result.ModregningUnderskud));
		}
	}

	// TODO: UdnyttetUnderskud = ModregningSkattepligeIndkomster, UdnyttetSkattevaerdi = ModregningSkatter.Sum()
	// TODO: Ikke udnyttede værdier for begge
	// NOTE: Restunderskud forener underskud og underskudsværdi
	/// <summary>
	/// Resultat af samlet modregning i egen skattepligtige indkomst og skatter samt ægtefælles skatepligtige indkomst og skatter.
	/// </summary>
	public class ModregnUnderskudResult
	{
		public ModregnUnderskudResult(decimal skattepligtigIndkomst, decimal modregningSkattepligtigIndkomst,
		                              SkatterAfPersonligIndkomst skatter, SkatterAfPersonligIndkomst modregningSkatter,
		                              decimal underskud, decimal modregningUnderskud)
		{
			SkattepligtigIndkomst = skattepligtigIndkomst;
			ModregningSkattepligtigIndkomst = modregningSkattepligtigIndkomst;
			Skatter = skatter;
			ModregningSkatter = modregningSkatter;
			Underskud = underskud;
			ModregningUnderskud = modregningUnderskud;
		}

		/// <summary>
		/// Størrelsen på den modregning, der kan rummes i den skattepligtige indkomst.
		/// </summary>
		public decimal ModregningSkattepligtigIndkomst { get; private set; }
			
		/// <summary>
		/// Den skattepligtige indkomst før modregning.
		/// </summary>
		public decimal SkattepligtigIndkomst { get; private set; }
			
		/// <summary>
		/// Den skattepligtige indkomst efter modregning.
		/// </summary>
		public decimal ModregnetSkattepligtigIndkomst
		{
			get { return SkattepligtigIndkomst - ModregningSkattepligtigIndkomst; }
		}

		/// <summary>
		/// Størrelsen af de modregninger af skatteværdier, der kan rummes i skatterne.
		/// </summary>
		public SkatterAfPersonligIndkomst ModregningSkatter { get; private set; }

		/// <summary>
		/// Skatterne inden modregning af underskudsværdi.
		/// </summary>
		public SkatterAfPersonligIndkomst Skatter { get; private set; }

		/// <summary>
		/// Skatterne efter modregning af underskudsværdi.
		/// </summary>
		public SkatterAfPersonligIndkomst ModregnedeSkatter 
		{
			get { return Skatter - ModregningSkatter; }
		}

		/// <summary>
		/// Underskuddet inden modregning og fremførsel.
		/// </summary>
		public decimal Underskud { get; private set; }

		/// <summary>
		/// Den del af underskuddet, der er benyttet til modregning i skattepligtig indkomst og skatter.
		/// </summary>
		public decimal ModregningUnderskud { get; private set; }

		/// <summary>
		/// Den del af underskuddet, der skal fremføres til næste indkomstår.
		/// </summary>
		// TODO: Rename to UnderskudTilFremfoersel
		public decimal Restunderskud
		{
			get { return Underskud - ModregningUnderskud; }
		}
	}
}