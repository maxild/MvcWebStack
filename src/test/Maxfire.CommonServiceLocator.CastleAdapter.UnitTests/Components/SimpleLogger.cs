using System;

namespace Maxfire.CommonServiceLocator.CastleAdapter.UnitTests.Components
{
	public class SimpleLogger : ILogger
	{
		public void Log(string msg)
		{
			Console.WriteLine(msg);
		}
	}
}