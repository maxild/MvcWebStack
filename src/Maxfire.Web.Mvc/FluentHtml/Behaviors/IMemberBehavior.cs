namespace Maxfire.Web.Mvc.FluentHtml.Behaviors
{
	public interface IMemberBehavior : IBehaviorMarker
	{
		void Execute(IMemberElement element);
	}
}