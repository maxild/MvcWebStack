using System;

namespace Maxfire.Web.Mvc
{
	/// <summary>
	/// Signals that the action parameter is not required (i.e. optional)
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
	public class NonRequiredAttribute : Attribute
	{
	}
}