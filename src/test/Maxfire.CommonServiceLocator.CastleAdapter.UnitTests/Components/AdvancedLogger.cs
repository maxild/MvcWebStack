using System;

namespace Maxfire.CommonServiceLocator.CastleAdapter.UnitTests.Components
{
	public class AdvancedLogger : ILogger
	{
		public void Log(string msg)
		{
			Console.WriteLine("Log: {0}", msg);
		}
	}
}