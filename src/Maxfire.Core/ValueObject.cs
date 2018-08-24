using System;
using System.Collections.Generic;
using System.Reflection;

namespace Maxfire.Core
{
	public abstract class ValueObject<T> : IEquatable<T> where T : class
	{
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			var other = obj as T;

			return Equals(other);
		}

		public override int GetHashCode()
		{
			IEnumerable<FieldInfo> fields = GetFields();

			const int START_VALUE = 17;
			const int MULTIPLIER = 59;

			int hashCode = START_VALUE;

			foreach (FieldInfo field in fields)
			{
				object value = field.GetValue(this);

				if (value != null)
					hashCode = hashCode * MULTIPLIER + value.GetHashCode();
			}

			return hashCode;
		}

		public virtual bool Equals(T other)
		{
			if (other == null)
				return false;

			Type t = GetType();

			FieldInfo[] fields = t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

			foreach (FieldInfo field in fields)
			{
				object otherValue = field.GetValue(other);
				object thisValue = field.GetValue(this);

				if (otherValue == null)
				{
					if (thisValue != null)
						return false;
				}
				else if (typeof(DateTime).IsAssignableFrom(field.FieldType) ||
				         typeof(DateTime?).IsAssignableFrom(field.FieldType))
				{
					string otherDateString = ((DateTime)otherValue).ToLongDateString();
					string thisDateString = ((DateTime)thisValue).ToLongDateString();
					if (!otherDateString.Equals(thisDateString))
					{
						return false;
					}
				}
				else if (!otherValue.Equals(thisValue))
					return false;
			}

			return true;
		}

		private IEnumerable<FieldInfo> GetFields()
		{
			Type t = GetType();

			var fields = new List<FieldInfo>();

			while (t != null && t != typeof(object))
			{
				fields.AddRange(t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));

				t = t.BaseType;
			}

			return fields;
		}

		public static bool operator ==(ValueObject<T> x, ValueObject<T> y)
		{
			if (ReferenceEquals(x, null))
				return ReferenceEquals(y, null);

			return x.Equals(y);
		}

		public static bool operator !=(ValueObject<T> x, ValueObject<T> y)
		{
			return !(x == y);
		}
	}
}
