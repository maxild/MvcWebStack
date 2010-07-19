using System;
using System.Collections.Generic;
using Maxfire.TestCommons.AssertExtensibility;
using Xunit;
using Xunit.Sdk;

namespace Maxfire.TestCommons.AssertExtensions
{
	/// <summary>
	/// Extensions which provide assertions to classes derived from <see cref="object"/>.
	/// </summary>
	public static class ObjectAssertExtensions
	{
		/// <summary>
		/// Verifies that a value is within a given range.
		/// </summary>
		/// <typeparam name="T">The type of the value to be compared</typeparam>
		/// <param name="actual">The actual value to be evaluated</param>
		/// <param name="low">The (inclusive) low value of the range</param>
		/// <param name="high">The (inclusive) high value of the range</param>
		/// <exception cref="InRangeException">Thrown when the value is not in the given range</exception>
		public static T ShouldBeInRange<T>(this T actual,
		                                   T low,
		                                   T high) where T : IComparable
		{
			Assert.InRange(actual, low, high);
			return actual;
		}

		/// <summary>
		/// Verifies that a value is within a given range, using a comparer.
		/// </summary>
		/// <typeparam name="T">The type of the value to be compared</typeparam>
		/// <param name="actual">The actual value to be evaluated</param>
		/// <param name="low">The (inclusive) low value of the range</param>
		/// <param name="high">The (inclusive) high value of the range</param>
		/// <param name="comparer">The comparer used to evaluate the value's range</param>
		/// <exception cref="InRangeException">Thrown when the value is not in the given range</exception>
		public static T ShouldBeInRange<T>(this T actual,
		                                   T low,
		                                   T high,
		                                   IComparer<T> comparer)
		{
			Assert.InRange(actual, low, high, comparer);
			return actual;
		}

		/// <summary>
		/// Verifies that an object reference is null.
		/// </summary>
		/// <param name="object">The object to be inspected</param>
		/// <exception cref="NullException">Thrown when the object reference is not null</exception>
		public static void ShouldBeNull(this object @object)
		{
			Assert.Null(@object);
		}

		/// <summary>
		/// Verifies that two objects are the same instance.
		/// </summary>
		/// <param name="actual">The actual object instance</param>
		/// <param name="expected">The expected object instance</param>
		/// <exception cref="SameException">Thrown when the objects are not the same instance</exception>
		public static void ShouldBeSameAs(this object actual,
		                                  object expected)
		{
			Assert.Same(expected, actual);
		}

		/// <summary>
		/// Verifies that an object is exactly the given type (and not a derived type).
		/// </summary>
		/// <typeparam name="T">The type the object should be</typeparam>
		/// <param name="object">The object to be evaluated</param>
		/// <returns>The object, casted to type T when successful</returns>
		/// <exception cref="IsTypeException">Thrown when the object is not the given type</exception>
		public static T ShouldBeOfType<T>(this object @object)
		{
			return Assert.IsType<T>(@object);
		}

		/// <summary>
		/// Verifies that an object is exactly the given type (and not a derived type).
		/// </summary>
		/// <param name="object">The object to be evaluated</param>
		/// <param name="expectedType">The type the object should be</param>
		/// <exception cref="IsTypeException">Thrown when the object is not the given type</exception>
		public static void ShouldBeOfType(this object @object,
		                                  Type expectedType)
		{
			Assert.IsType(expectedType, @object);
		}

		public static void ShouldBeAssignableFrom<T>(this object @object)
		{
			Assert.IsAssignableFrom<T>(@object);
		}

		public static void ShouldBeAssignableFrom(this object @object, Type expectedType)
		{
			Assert.IsAssignableFrom(expectedType, @object);
		}

		public static void ShouldBeAssignableTo<T>(this object @object)
		{
			@object.ShouldBeAssignableTo(typeof (T));
		}

		public static void ShouldBeAssignableTo(this object @object, Type expectedType)
		{
			expectedType.IsAssignableFrom(@object.GetType());
		}

		/// <summary>
		/// Verifies that two objects are equal (by type).
		/// </summary>
		/// <typeparam name="T">The type of the objects to be compared</typeparam>
		/// <param name="actual">The value to be compared against</param>
		/// <param name="expected">The expected value</param>
		/// <exception cref="EqualException">Thrown when the objects are not equal</exception>
		public static T ShouldEqual<T>(this T actual,
		                               T expected)
		{
			Assert.Equal(expected, actual, new AssertEqualityComparer<T>());
			return actual;
		}

		public static T ShouldEqualDefault<T>(this T actual) where T : new()
		{
			Assert.Equal(new T(), actual, new AssertEqualityComparer<T>());
			return actual;
		}

		/// <summary>
		/// Verifies that two objects are equal (by type).
		/// </summary>
		/// <typeparam name="T">The type of the objects to be compared</typeparam>
		/// <param name="actual">The value to be compared against</param>
		/// <param name="expected">The expected value</param>
		/// <exception cref="EqualException">Thrown when the objects are not equal</exception>
		public static T ShouldEqual<T>(this T actual,
		                               object expected)
		{
			Assert.Equal(expected, actual, new AssertEqualityComparer<object>());
			return actual;
		}

		/// <summary>
		/// Verifies that two objects are equal, using a custom comparer.
		/// </summary>
		/// <typeparam name="T">The type of the objects to be compared</typeparam>
		/// <param name="actual">The value to be compared against</param>
		/// <param name="expected">The expected value</param>
		/// <param name="comparer">The comparer used to compare the two objects</param>
		/// <exception cref="EqualException">Thrown when the objects are not equal</exception>
		public static T ShouldEqual<T>(this T actual,
		                               T expected,
		                               IEqualityComparer<T> comparer)
		{
			Assert.Equal(expected, actual, comparer);
			return actual;
		}

		/// <summary>
		/// Verifies that a value is not within a given range, using the default comparer.
		/// </summary>
		/// <typeparam name="T">The type of the value to be compared</typeparam>
		/// <param name="actual">The actual value to be evaluated</param>
		/// <param name="low">The (inclusive) low value of the range</param>
		/// <param name="high">The (inclusive) high value of the range</param>
		/// <exception cref="NotInRangeException">Thrown when the value is in the given range</exception>
		public static T ShouldNotBeInRange<T>(this T actual,
		                                      T low,
		                                      T high) where T : IComparable
		{
			Assert.NotInRange(actual, low, high);
			return actual;
		}

		/// <summary>
		/// Verifies that a value is not within a given range, using a comparer.
		/// </summary>
		/// <typeparam name="T">The type of the value to be compared</typeparam>
		/// <param name="actual">The actual value to be evaluated</param>
		/// <param name="low">The (inclusive) low value of the range</param>
		/// <param name="high">The (inclusive) high value of the range</param>
		/// <param name="comparer">The comparer used to evaluate the value's range</param>
		/// <exception cref="NotInRangeException">Thrown when the value is in the given range</exception>
		public static T ShouldNotBeInRange<T>(this T actual,
		                                      T low,
		                                      T high,
		                                      IComparer<T> comparer)
		{
			Assert.NotInRange(actual, low, high, comparer);
			return actual;
		}

		/// <summary>
		/// Verifies that an object reference is not null.
		/// </summary>
		/// <param name="object">The object to be validated</param>
		/// <exception cref="NotNullException">Thrown when the object is not null</exception>
		public static void ShouldNotBeNull(this object @object)
		{
			Assert.NotNull(@object);
		}

		/// <summary>
		/// Verifies that an object reference is not null.
		/// </summary>
		/// <param name="object">The object to be validated</param>
		/// <exception cref="NotNullException">Thrown when the object is not null</exception>
		public static T ShouldNotBeNull<T>(this T @object)
		{
			Assert.NotNull(@object);
			return @object;
		}


		/// <summary>
		/// Verifies that two objects are not the same instance.
		/// </summary>
		/// <param name="actual">The actual object instance</param>
		/// <param name="expected">The expected object instance</param>
		/// <exception cref="NotSameException">Thrown when the objects are the same instance</exception>
		public static void ShouldNotBeSameAs(this object actual,
		                                     object expected)
		{
			Assert.NotSame(expected, actual);
		}

		/// <summary>
		/// Verifies that an object is not exactly the given type.
		/// </summary>
		/// <typeparam name="T">The type the object should not be</typeparam>
		/// <param name="object">The object to be evaluated</param>
		/// <exception cref="IsTypeException">Thrown when the object is the given type</exception>
		public static void ShouldNotBeOfType<T>(this object @object)
		{
			Assert.IsNotType<T>(@object);
		}

		/// <summary>
		/// Verifies that an object is not exactly the given type.
		/// </summary>
		/// <param name="object">The object to be evaluated</param>
		/// <param name="expectedType">The type the object should not be</param>
		/// <exception cref="IsTypeException">Thrown when the object is the given type</exception>
		public static void ShouldNotBeOfType(this object @object,
		                                     Type expectedType)
		{
			Assert.IsNotType(expectedType, @object);
		}

		/// <summary>
		/// Verifies that two objects are not equal, using a default comparer.
		/// </summary>
		/// <typeparam name="T">The type of the objects to be compared</typeparam>
		/// <param name="actual">The actual object</param>
		/// <param name="expected">The expected object</param>
		/// <exception cref="NotEqualException">Thrown when the objects are equal</exception>
		public static T ShouldNotEqual<T>(this T actual,
		                                  T expected)
		{
			Assert.NotEqual(expected, actual, new AssertEqualityComparer<T>());
			return actual;
		}

		/// <summary>
		/// Verifies that two objects are not equal, using a default comparer.
		/// </summary>
		/// <typeparam name="T">The type of the objects to be compared</typeparam>
		/// <param name="actual">The actual object</param>
		/// <param name="expected">The expected object</param>
		/// <exception cref="NotEqualException">Thrown when the objects are equal</exception>
		public static T ShouldNotEqual<T>(this T actual,
		                                  object expected)
		{
			Assert.NotEqual(expected, actual, new AssertEqualityComparer<object>());
			return actual;
		}

		/// <summary>
		/// Verifies that two objects are not equal, using a custom comparer.
		/// </summary>
		/// <typeparam name="T">The type of the objects to be compared</typeparam>
		/// <param name="actual">The actual object</param>
		/// <param name="expected">The expected object</param>
		/// <param name="comparer">The comparer used to examine the objects</param>
		/// <exception cref="NotEqualException">Thrown when the objects are equal</exception>
		public static T ShouldNotEqual<T>(this T actual,
		                                  T expected,
		                                  IEqualityComparer<T> comparer)
		{
			Assert.NotEqual(expected, actual, comparer);
			return actual;
		}
	}
}