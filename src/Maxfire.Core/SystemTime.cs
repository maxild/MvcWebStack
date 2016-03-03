using System;

namespace Maxfire.Core
{
	/// <summary>
	/// A system time (clock) that can easily be 'faked' during testing
	/// </summary>
	public static class SystemTime
	{
		/// <summary>
		/// The date and time.
		/// </summary>
		public static Func<DateTime> Now = () => DateTime.Now;

		/// <summary>
		/// The date component
		/// </summary>
		public static Func<DateTime> Today = () => DateTime.Today;
	}
}