namespace Maxfire.Web.Mvc
{
	public abstract class EntityViewModel<TId> : IEntityViewModel<TId>
	{
		public TId Id { get; set; }
		public bool IsTransient
		{
			get
			{
				return Id.Equals(default(TId));
			}
		}
	}
}