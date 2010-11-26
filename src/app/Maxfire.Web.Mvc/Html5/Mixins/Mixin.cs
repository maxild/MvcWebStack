namespace Maxfire.Web.Mvc.Html5.Mixins
{
	/// <summary>
	/// Base class for all mixin classes.
	/// </summary>
	public abstract class Mixin<T> where T : class
	{
		protected T self { get { return this as T; } }
	}
}