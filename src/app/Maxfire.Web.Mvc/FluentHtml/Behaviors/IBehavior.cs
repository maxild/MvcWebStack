namespace Maxfire.Web.Mvc.FluentHtml.Behaviors
{
	public interface IBehavior : IBehaviorMarker
	{
		void Execute(IElement element);
	}
}