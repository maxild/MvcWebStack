using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Maxfire.Core;
using Maxfire.Core.Extensions;

namespace Maxfire.Skat
{
	public class ValueTupple<T> : IEnumerable<T>, IEquatable<ValueTupple<T>>
	{
		private readonly IList<T> _list;

		public ValueTupple(T first)
		{
			_list = new List<T> { first };
		}

		public ValueTupple(T first, T second)
		{
			_list = new List<T> { first, second };
		}

		public ValueTupple(IList<T> list)
		{
			list.ThrowIfNull("list");
			if (list.Count != 1 && list.Count != 2)
			{
				throw new ArgumentException("The list must contain either one or two elements");
			}
			_list = list;
		}

		public override string ToString()
		{
			return "(" + string.Join(", ", _list.Select(x => x.ToString()).ToArray()) + ")";
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = 1;
				for (int i = 0; i < Size; i++)
				{
					hashCode = (hashCode * 397) ^ this[i].GetHashCode();
				}
				return hashCode;
			}
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as ValueTupple<T>);
		}

		public bool Equals(ValueTupple<T> other)
		{
			if (other == null)
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			if (Size != other.Size)
			{
				return false;
			}

			for (int i = 0; i < Size; i++)
			{
				if (false == this[i].Equals(other[i]))
				{
					return false;
				}
			}

			return true;
		}

		public T this[int index]
		{
			get { return _list[index]; }
		}

		public int Size
		{
			get { return _list.Count; }
		}

		public int IndexOf(T value)
		{
			return _list.IndexOf(value);
		}

		public T PartnerOf(T partner)
		{
			int i = _list.IndexOf(partner);
			if (i == 0 || i == 1)
			{
				return _list[i == 0 ? 1 : 0];
			}
			return default(T);
		}

		public T Sum()
		{
			T total = Operator<T>.Zero;
			for (int i = 0; i < Size; i++)
			{
				total = Operator<T>.Add(total, this[i]);
			}
			return total;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// The unary plus (+) operator is overloaded to mean NonNegative.
		/// </summary>
		public static ValueTupple<T> operator +(ValueTupple<T> tupple)
		{
			return unaryOp(tupple, x =>
			                       	{
			                       		T zero = Operator<T>.Zero;
			                       		if (Operator<T>.LessThan(x, zero))
			                       		{
			                       			return zero;
			                       		}
			                       		return x;
			                       	});
		}

		public static ValueTupple<T> operator -(ValueTupple<T> tupple)
		{
			return unaryOp(tupple, Operator<T>.Negate);
		}

		private static ValueTupple<T> unaryOp(ValueTupple<T> tupple, Func<T, T> op)
		{
			var list = new List<T>(tupple.Size);
			for (int i = 0; i < tupple.Size; i++)
			{
				list.Add(op(tupple[i]));
			}

			return new ValueTupple<T>(list);
		}

		public static ValueTupple<T> operator +(ValueTupple<T> lhs, ValueTupple<T> rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Add);
		}

		public static ValueTupple<T> operator +(T lhs, ValueTupple<T> rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Add);
		}

		public static ValueTupple<T> operator +(ValueTupple<T> lhs, T rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Add);
		}

		public static ValueTupple<T> operator -(ValueTupple<T> lhs, ValueTupple<T> rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Subtract);
		}
		
		public static ValueTupple<T> operator -(T lhs, ValueTupple<T> rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Subtract);
		}

		public static ValueTupple<T> operator -(ValueTupple<T> lhs, T rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Subtract);
		}

		public static ValueTupple<T> operator *(ValueTupple<T> lhs, ValueTupple<T> rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Multiply);
		}

		public static ValueTupple<T> operator *(T lhs, ValueTupple<T> rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Multiply);
		}

		public static ValueTupple<T> operator *(ValueTupple<T> lhs, T rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Multiply);
		}
		
		public static ValueTupple<T> operator /(ValueTupple<T> lhs, ValueTupple<T> rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Divide);
		}

		public static ValueTupple<T> operator /(T lhs, ValueTupple<T> rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Divide);
		}

		public static ValueTupple<T> operator /(ValueTupple<T> lhs, T rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Divide);
		}

		private static ValueTupple<T> binaryOp(ValueTupple<T> lhs, ValueTupple<T> rhs, Func<T, T, T> op)
		{
			if (lhs.Size != rhs.Size)
			{
				throw new ArgumentException("Cannot add tupples of different size.");
			}

			var list = new List<T>(lhs.Size);
			for (int i = 0; i < lhs.Size; i++)
			{
				list.Add(op(lhs[i], rhs[i]));
			}

			return new ValueTupple<T>(list);
		}

		private static ValueTupple<T> binaryOp(T lhs, ValueTupple<T> rhs, Func<T, T, T> op)
		{
			var list = new List<T>(rhs.Size);
			for (int i = 0; i < rhs.Size; i++)
			{
				list.Add(op(lhs, rhs[i]));
			}

			return new ValueTupple<T>(list);
		}

		private static ValueTupple<T> binaryOp(ValueTupple<T> lhs, T rhs, Func<T, T, T> op)
		{
			var list = new List<T>(lhs.Size);
			for (int i = 0; i < lhs.Size; i++)
			{
				list.Add(op(lhs[i], rhs));
			}

			return new ValueTupple<T>(list);
		}
	}
}