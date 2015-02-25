using System;
using System.Globalization;
using System.Threading;

namespace Maxfire.TestCommons
{
	public class CurrentCultureScope : IDisposable
	{
		private readonly CultureInfo _culture;
		private readonly CultureInfo _uiCulture;

		public CurrentCultureScope(string name)
		{
			_culture = Thread.CurrentThread.CurrentCulture;
			_uiCulture = Thread.CurrentThread.CurrentUICulture;
			Thread.CurrentThread.CurrentCulture = new CultureInfo(name);
			Thread.CurrentThread.CurrentUICulture = new CultureInfo(name);
		}

		public void Dispose()
		{
			Thread.CurrentThread.CurrentCulture = _culture;
			Thread.CurrentThread.CurrentUICulture = _uiCulture;
		}
	}
}