using System;

namespace Quarks
{
	/// <summary>
	/// This is a trivial class that is used to make sure that Equals and GetHashCode
	/// are properly overloaded with the correct semantics. This is extremely important
	/// if you are going to deal with objects outside the current Unit of Work.
	/// </summary>
	/// <typeparam name="TEntity">Entity type</typeparam>
	/// <typeparam name="TKey">Identity key type</typeparam>
	[Serializable]
	public abstract class IdentityFieldProvider<TEntity, TKey> : IEntity
		where TEntity : IdentityFieldProvider<TEntity, TKey>
	{
		int? _oldHashCode;

		/// <summary>
		/// Gets or sets the Id of this entity
		/// </summary>
		public virtual TKey Id { get; set; }

		object IEntity.Id
		{
			get { return Id; }
		}

		public virtual bool IsTransient()
		{
			return Equals(Id, default(TKey));
		}

		/// <summary>
		/// Equality operator so we can have == semantics
		/// </summary>
		/// <returns>Equality of instances.</returns>
		/// <param name="left">Left instance to be compared</param>
		/// <param name="right">Right instance to be compared</param>
		public static bool operator ==(IdentityFieldProvider<TEntity, TKey> left, IdentityFieldProvider<TEntity, TKey> right)
		{
			return Equals(left, right);
		}

		/// <summary>
		/// Inequality operator so we can have != semantics
		/// </summary>
		/// <returns>Inequality of instances.</returns>
		/// <param name="left">Left instance to be compared</param>
		/// <param name="right">Right instance to be compared</param>
		public static bool operator !=(IdentityFieldProvider<TEntity, TKey> left, IdentityFieldProvider<TEntity, TKey> right)
		{
			return !(left == right);
		}

		public override bool Equals(object obj)
		{
			var other = obj as TEntity;
			if (other == null)
				return false;

			// to handle the case of comparing two new objects
			if (other.IsTransient() && IsTransient())
				return ReferenceEquals(other, this);

			return other.Id.Equals(Id);
		}

		public override int GetHashCode()
		{
			// This is done se we won't change the hash code
			// ReSharper disable NonReadonlyFieldInGetHashCode
			if (_oldHashCode.HasValue)
				return _oldHashCode.Value;

			// When we are transient, we use the base GetHashCode()
			// and remember it, so an instance can't change its hash code.
			if (IsTransient())
			{
				_oldHashCode = base.GetHashCode();
				return _oldHashCode.Value;
			}
			// ReSharper restore NonReadonlyFieldInGetHashCode

			return Id.GetHashCode();
		}
	}
}
