using System;
using System.Web.Mvc;

namespace Maxfire.Web.Mvc.ValueProviders
{
	public class GetValueProviderUsing : IValueProviderFactory
	{
		private readonly Func<ControllerContext, IValueProvider> _thunk;

		public GetValueProviderUsing(Func<ControllerContext, IValueProvider> thunk)
		{
			if (thunk == null)
				throw new ArgumentNullException(nameof(thunk));
			_thunk = thunk;
		}

		public IValueProvider GetValueProvider(ControllerContext controllerContext)
		{
			return _thunk(controllerContext);
		}
	}
}
