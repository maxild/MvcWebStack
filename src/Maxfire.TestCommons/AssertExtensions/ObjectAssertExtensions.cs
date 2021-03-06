using System;
using System.Collections.Generic;
using Maxfire.Core;
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

		public static T ShouldBeGreaterThanZero<T>(this T actual)
		{
			Assert.True(Operator<T>.GreaterThanZero(actual));
			return actual;
		}

		public static T ShouldBeLessThanZero<T>(this T actual)
		{
			Assert.True(Operator<T>.LessThanZero(actual));
			return actual;
		}

		public static T ShouldBeLessThanOrEqualToZero<T>(this T actual)
		{
			Assert.True(Operator<T>.LessThanOrEqualToZero(actual));
			return actual;
		}

		public static T ShouldBeGreaterThanOrEqualToZero<T>(this T actual)
		{
			Assert.True(Operator<T>.GreaterThanOrEqualToZero(actual));
			return actual;
		}

		public static T ShouldBeGreaterThan<T>(this T actual, T lowerBound)
		{
			Assert.True(Operator<T>.GreaterThan(actual, lowerBound));
			return actual;
		}

		public static T ShouldBeLessThan<T>(this T actual, T upperBound)
		{
			Assert.True(Operator<T>.LessThan(actual, upperBound));
			return actual;
		}

		public static T ShouldBeLessThanOrEqualTo<T>(this T actual, T upperBound)
		{
			Assert.True(Operator<T>.LessThanOrEqual(actual, upperBound));
			return actual;
		}

		public static T ShouldBeGreaterThanOrEqualTo<T>(this T actual, T lowerBound)
		{
			Assert.True(Operator<T>.GreaterThanOrEqual(actual, lowerBound));
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

		public static void ShouldBeDerivedFromType<T>(this object @object)
		{
			Assert.IsAssignableFrom<T>(@object);
		}

		public static void ShouldBeDerivedFromType(this object @object, Type expectedType)
		{
			Assert.IsAssignableFrom(expectedType, @object);
		}

		/// <summary>
		/// Verifies that two objects are equal (using default comparer).
		/// </summary>
		/// <typeparam name="T">The type of the objects to be compared</typeparam>
		/// <param name="actual">The value to be compared against</param>
		/// <param name="expected">The expected value</param>
		/// <param name="skipTypeCheck">If true, skip checking that actual and expected values have the same type.</param>
		/// <exception cref="EqualException">Thrown when the objects are not equal</exception>
		public static T ShouldEqual<T>(this T actual, T expected, bool skipTypeCheck = false)
		{
			Assert.Equal(expected, actual, new AssertEqualityComparer<T>(skipTypeCheck));
			return actual;
		}

		/// <summary>
		/// Verifies that two objects are equal (using dynamic comparer).
		/// </summary>
		/// <typeparam name="T">The type of the objects to be compared</typeparam>
		/// <param name="actual">The value to be compared against</param>
		/// <param name="expected">The expected value</param>
		/// <param name="skipTypeCheck">If true, skip checking that actual and expected values have the same type.</param>
		/// <exception cref="EqualException">Thrown when the objects are not equal</exception>
		public static T ShouldEqual<T>(this T actual, object expected, bool skipTypeCheck = true)
		{
			Assert.Equal(expected, actual, new DynamicAssertEqualityComparer(skipTypeCheck));
			return actual;
		}

		public static T ShouldStrictEqual<T>(this T actual, object expected)
		{
			Assert.Equal(expected, actual, new DynamicAssertEqualityComparer());
			return actual;
		}

		public static T ShouldEqualDefault<T>(this T actual) where T : new()
		{
			Assert.Equal(new T(), actual, new AssertEqualityComparer<T>());
			return actual;
		}

		/// <summary>
		/// Verifies that two objects are equal (using custom comparer).
		/// </summary>
		/// <typeparam name="T">The type of the objects to be compared</typeparam>
		/// <param name="actual">The value to be compared against</param>
		/// <param name="expected">The expected value</param>
		/// <param name="comparer">The comparer used to compare the two objects</param>
		/// <exception cref="EqualException">Thrown when the objects are not equal</exception>
		public static T ShouldEqual<T>(this T actual, T expected, IEqualityComparer<T> comparer)
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
		/// Verifies that two objects are not equal (using default comparer).
		/// </summary>
		/// <typeparam name="T">The type of the objects to be compared</typeparam>
		/// <param name="actual">The actual object</param>
		/// <param name="expected">The expected object</param>
		/// <param name="skipTypeCheck">If true, skip checking that actual and expected values have the same type.</param>
		/// <exception cref="NotEqualException">Thrown when the objects are equal</exception>
		public static T ShouldNotEqual<T>(this T actual, T expected, bool skipTypeCheck = false)
		{
			Assert.NotEqual(expected, actual, new AssertEqualityComparer<T>(skipTypeCheck));
			return actual;
		}

		/// <summary>
		/// Verifies that two objects are not equal (using dynamic comparer).
		/// </summary>
		/// <typeparam name="T">The type of the objects to be compared</typeparam>
		/// <param name="actual">The actual object</param>
		/// <param name="expected">The expected object</param>
		/// <param name="skipTypeCheck">If true, skip checking that actual and expected values have the same type.</param>
		/// <exception cref="NotEqualException">Thrown when the objects are equal</exception>
		public static T ShouldNotEqual<T>(this T actual, object expected, bool skipTypeCheck = true)
		{
			Assert.NotEqual(expected, actual, new DynamicAssertEqualityComparer(skipTypeCheck));
			return actual;
		}

		public static T ShouldNotStrictEqual<T>(this T actual, object expected)
		{
			Assert.NotEqual(expected, actual, new DynamicAssertEqualityComparer());
			return actual;
		}

		/// <summary>
		/// Verifies that two objects are not equal (using custom comparer).
		/// </summary>
		/// <typeparam name="T">The type of the objects to be compared</typeparam>
		/// <param name="actual">The actual object</param>
		/// <param name="expected">The expected object</param>
		/// <param name="comparer">The comparer used to examine the objects</param>
		/// <exception cref="NotEqualException">Thrown when the objects are equal</exception>
		public static T ShouldNotEqual<T>(this T actual, T expected, IEqualityComparer<T> comparer)
		{
			Assert.NotEqual(expected, actual, comparer);
			return actual;
		}
	}
}