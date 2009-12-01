using System;

namespace Maxfire.Core
{
	public abstract class Entity : Entity<long>
	{
	}

	public abstract class Entity<TId> : IEntity<TId>
	{
		private int? _oldHashCode;
		private readonly TId _id;

		protected Entity()
		{
			_id = default(TId);
		}

		protected Entity(TId id)
		{
			_id = id;
		}

		public virtual TId Id
		{
			get { return _id; }
		}

		public virtual bool IsTransient
		{
			get { return Equals(Id, default(TId)); }
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			Entity<TId> other = obj as Entity<TId>;

			if (other == null || !GetType().Equals(other.GetTypeUnproxied()))
			{
				return false;
			}
			
			if (other.IsTransient && IsTransient)
			{
				return ReferenceEquals(other, this);
			}

			return other.Id.Equals(Id);
		}

		public override int GetHashCode()
		{
			return _oldHashCode ?? getHashCode();
		}

		protected virtual Type GetTypeUnproxied()
		{
			return GetType();
		}

		private int getHashCode()
		{
			if (IsTransient)
			{
				_oldHashCode = base.GetHashCode();
				return _oldHashCode.Value;
			}

			return Id.GetHashCode();
		}
	}
}