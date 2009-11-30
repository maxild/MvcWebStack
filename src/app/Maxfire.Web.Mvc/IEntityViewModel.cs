namespace Maxfire.Web.Mvc
{
	public interface IEntityViewModel<TId>
	{
		TId Id { get; }
		bool IsTransient { get; }
	}
}