namespace Maxfire.Core
{
	public interface IEntity<TId>
	{
		TId Id { get; }
		bool IsTransient { get; }
	}
}