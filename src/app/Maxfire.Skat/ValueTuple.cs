using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Maxfire.Core;
using Maxfire.Core.Extensions;

namespace Maxfire.Skat
{
	/// <summary>
	/// In mathematics and computer science a tuple represents the notion of an ordered list of 
	/// elements. In set theory, an (ordered) n-tuple is a sequence (or ordered list) of n elements, 
	/// where n is a positive integer. There is also one 0-tuple, an empty sequence. An n-tuple is 
	/// defined inductively using the construction of an ordered pair.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ValueTuple<T> : IEnumerable<T>, IEquatable<ValueTuple<T>>
	{
		private readonly IList<T> _list;

		public ValueTuple(T first)
		{
			_list = new List<T> { first };
		}

		public ValueTuple(T first, T second)
		{
			_list = new List<T> { first, second };
		}

		public ValueTuple(int size, Func<T> creator)
		{
			if (size != 1 && size != 2)
			{
				throw new ArgumentException("A ValueTuple must contain either one or two elements");
			}
			_list = new List<T>(size);
			for (int i = 0; i < size; i++)
			{
				_list.Add(creator());
			}
		}

		public ValueTuple(IList<T> list)
		{
			list.ThrowIfNull("list");
			if (list.Count != 1 && list.Count != 2)
			{
				throw new ArgumentException("A ValueTuple must contain either one or two elements");
			}
			_list = list;
		}

		/// <summary>
		/// Tuples are usually written by listing the elements within parentheses '( )' and 
		/// separated by commas; for example, (2, 7, 4, 1, 7) denotes a 5-tuple.
		/// </summary>
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
			return Equals(obj as ValueTuple<T>);
		}

		public bool Equals(ValueTuple<T> other)
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

		public bool AllZero()
		{
			for (int i = 0; i < Size; i++)
			{
				if (Operator<T>.Equal(this[i], Operator<T>.Zero))
				{
					return true;
				}
			}
			return false;
		}

		public int IndexOf(T value)
		{
			return _list.IndexOf(value);
		}

		public int IndexOf(Func<T, bool> predicate)
		{
			for (int i = 0; i < Size; i++)
			{
				if (predicate(this[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public T PartnerOf(int index)
		{
			if (index == 0 || index == 1)
			{
				return _list[index == 0 ? 1 : 0];
			}
			throw new IndexOutOfRangeException();
		}

		public T PartnerOf(T partner)
		{
			int i = _list.IndexOf(partner);
			if (i >= 0)
			{
				return _list[i == 0 ? 1 : 0];
			}
			throw new ArgumentException("The object cannot be found in the tuple.");
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

		public ValueTuple<T> Swap()
		{
			return Size > 1 ? new ValueTuple<T>(this[1], this[0]) : this;
		}

		public ValueTuple<T> Clone()
		{
			var list = new List<T>(Size);
			for (int i = 0; i < Size; i++)
			{
				var item = _list[i];
				var cloneable = item as ICloneable;
				if (cloneable != null)
				{
					item = (T)cloneable.Clone();
				}
				list.Add(item);
			}
			return new ValueTuple<T>(list);
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
		public static ValueTuple<T> operator +(ValueTuple<T> tuple)
		{
			return unaryOp(tuple, x =>
			                       	{
			                       		T zero = Operator<T>.Zero;
			                       		if (Operator<T>.LessThan(x, zero))
			                       		{
			                       			return zero;
			                       		}
			                       		return x;
			                       	});
		}

		public static ValueTuple<T> operator -(ValueTuple<T> tuple)
		{
			return unaryOp(tuple, Operator<T>.Negate);
		}

		private static ValueTuple<T> unaryOp(ValueTuple<T> tuple, Func<T, T> op)
		{
			var list = new List<T>(tuple.Size);
			for (int i = 0; i < tuple.Size; i++)
			{
				list.Add(op(tuple[i]));
			}

			return new ValueTuple<T>(list);
		}

		public static ValueTuple<T> operator +(ValueTuple<T> lhs, ValueTuple<T> rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Add, "add");
		}

		public static ValueTuple<T> operator +(T lhs, ValueTuple<T> rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Add);
		}

		public static ValueTuple<T> operator +(ValueTuple<T> lhs, T rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Add);
		}

		public static ValueTuple<T> operator -(ValueTuple<T> lhs, ValueTuple<T> rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Subtract, "subtract");
		}
		
		public static ValueTuple<T> operator -(T lhs, ValueTuple<T> rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Subtract);
		}

		public static ValueTuple<T> operator -(ValueTuple<T> lhs, T rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Subtract);
		}

		public static ValueTuple<T> operator *(ValueTuple<T> lhs, ValueTuple<T> rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Multiply, "multiply");
		}

		public static ValueTuple<T> operator *(T lhs, ValueTuple<T> rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Multiply);
		}

		public static ValueTuple<T> operator *(ValueTuple<T> lhs, T rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Multiply);
		}
		
		public static ValueTuple<T> operator /(ValueTuple<T> lhs, ValueTuple<T> rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Divide, "divide");
		}

		public static ValueTuple<T> operator /(T lhs, ValueTuple<T> rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Divide);
		}

		public static ValueTuple<T> operator /(ValueTuple<T> lhs, T rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Divide);
		}

		private static ValueTuple<T> binaryOp(ValueTuple<T> lhs, ValueTuple<T> rhs, Func<T, T, T> op, string s)
		{
			if (lhs.Size != rhs.Size)
			{
				throw new ArgumentException(string.Format("Cannot {0} tuples of different size.", s));
			}

			var list = new List<T>(lhs.Size);
			for (int i = 0; i < lhs.Size; i++)
			{
				list.Add(op(lhs[i], rhs[i]));
			}

			return new ValueTuple<T>(list);
		}

		private static ValueTuple<T> binaryOp(T lhs, ValueTuple<T> rhs, Func<T, T, T> op)
		{
			var list = new List<T>(rhs.Size);
			for (int i = 0; i < rhs.Size; i++)
			{
				list.Add(op(lhs, rhs[i]));
			}

			return new ValueTuple<T>(list);
		}

		private static ValueTuple<T> binaryOp(ValueTuple<T> lhs, T rhs, Func<T, T, T> op)
		{
			var list = new List<T>(lhs.Size);
			for (int i = 0; i < lhs.Size; i++)
			{
				list.Add(op(lhs[i], rhs));
			}

			return new ValueTuple<T>(list);
		}
	}
}