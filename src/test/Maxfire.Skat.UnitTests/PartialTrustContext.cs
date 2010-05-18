using System;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;

namespace Maxfire.Skat.UnitTests
{
	public static class PartialTrustContext
	{
		public static void RunTest<TRunner>(Action<TRunner> testDriver, Action<PermissionSet> permissionsSetup = null)
		{
			var setup = new AppDomainSetup
			{
				ApplicationBase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
			};
			
			var permissions = new PermissionSet(null);
			permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
			if (permissionsSetup != null)
			{
				permissionsSetup(permissions);
			}
			
			AppDomain sandbox = null;
			try
			{
				sandbox = AppDomain.CreateDomain("sandbox", null, setup, permissions);
				var runner = (TRunner)sandbox.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName,
				                                                      typeof(TRunner).FullName);

				testDriver(runner);
			}
			finally
			{
				if (sandbox != null)
				{
					AppDomain.Unload(sandbox);
				}
			}
		}
	}
}