using Maxfire.TestCommons;
using Context = System.Diagnostics.CodeAnalysis.UsedImplicitlyAttribute;
using Specification = Xunit.FactAttribute;

namespace Maxfire.Web.Mvc.UnitTests
{
	[Context]
	public class when_model_binding_to_lists : XunitSpec
	{
		class Order
		{
			public Orderline[] Orderlines { get; set; }
		}

		class Orderline
		{
			public decimal Price { get; set; }

			public int Qty { get; set; }
		}

		TestableValidationModelBinder<Order> Binder { get; set; }

		protected override void Establish_context()
		{
			Binder = new TestableValidationModelBinder<Order>();
		}

		// Todo: make specs for indices with 'holes' and arbitrary indices
	}
}