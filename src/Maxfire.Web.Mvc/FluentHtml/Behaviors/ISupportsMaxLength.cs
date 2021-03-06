namespace Maxfire.Web.Mvc.FluentHtml.Behaviors
{
	/// <summary>
	/// Marker interface indicating that a particular element supports the maxlength HTML attribute.
	/// </summary>
	public interface ISupportsMaxLength
	{
		void SetMaxLength(int value);
	}
}