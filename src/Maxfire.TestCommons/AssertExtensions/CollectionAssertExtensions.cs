using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Maxfire.TestCommons.AssertExtensibility;
using Xunit;
using Xunit.Sdk;

namespace Maxfire.TestCommons.AssertExtensions
{
	/// <summary>
	/// Extensions which provide assertions to classes derived from <see cref="IEnumerable"/> and <see cref="IEnumerable&lt;T&gt;"/>.
	/// </summary>
	[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
	public static class CollectionAssertExtensions
	{
		public static IEnumerable ShouldHaveCount(this IEnumerable collection, int expectedCount)
		{
			var count = 0;
			var iter = collection.GetEnumerator();
			while (iter.MoveNext()) count++;
			count.ShouldEqual(expectedCount);
			return collection;
		}

		/// <summary>
		/// Verifies that a collection is empty.
		/// </summary>
		/// <param name="collection">The collection to be inspected</param>
		/// <exception cref="ArgumentNullException">Thrown when the collection is null</exception>
		/// <exception cref="EmptyException">Thrown when the collection is not empty</exception>
		public static IEnumerable ShouldBeEmpty(this IEnumerable collection)
		{
			Assert.Empty(collection);
			return collection;
		}

		/// <summary>
		/// Verifies that a collection is not empty.
		/// </summary>
		/// <param name="collection">The collection to be inspected</param>
		/// <exception cref="ArgumentNullException">Thrown when a null collection is passed</exception>
		/// <exception cref="NotEmptyException">Thrown when the collection is empty</exception>
		public static IEnumerable ShouldNotBeEmpty(this IEnumerable collection)
		{
			Assert.NotEmpty(collection);
			return collection;
		}

		/// <summary>
		/// Verifies that a collection contains a given object.
		/// </summary>
		/// <typeparam name="T">The type of the object to be verified</typeparam>
		/// <param name="collection">The collection to be inspected</param>
		/// <param name="expected">The object expected to be in the collection</param>
		/// <exception cref="ContainsException">Thrown when the object is not present in the collection</exception>
		public static IEnumerable<T> ShouldContain<T>(this IEnumerable<T> collection,
		                                              T expected)
		{
			Assert.Contains(expected, collection);
			return collection;
		}

		/// <summary>
		/// Verifies that a collection contains a given object, using a comparer.
		/// </summary>
		/// <typeparam name="T">The type of the object to be verified</typeparam>
		/// <param name="collection">The collection to be inspected</param>
		/// <param name="expected">The object expected to be in the collection</param>
		/// <param name="comparer">The comparer used to equate objects in the collection with the expected object</param>
		/// <exception cref="ContainsException">Thrown when the object is not present in the collection</exception>
		public static IEnumerable<T> ShouldContain<T>(this IEnumerable<T> collection,
		                                              T expected,
		                                              IEqualityComparer<T> comparer)
		{
			Assert.Contains(expected, collection, comparer);
			return collection;
		}

		/// <summary>
		/// Verifies that a collection does not contain a given object.
		/// </summary>
		/// <typeparam name="T">The type of the object to be compared</typeparam>
		/// <param name="expected">The object that is expected not to be in the collection</param>
		/// <param name="collection">The collection to be inspected</param>
		/// <exception cref="DoesNotContainException">Thrown when the object is present inside the container</exception>
		public static IEnumerable<T> ShouldNotContain<T>(this IEnumerable<T> collection,
		                                                 T expected)
		{
			Assert.DoesNotContain(expected, collection);
			return collection;
		}

		/// <summary>
		/// Verifies that a collection does not contain a given object, using a comparer.
		/// </summary>
		/// <typeparam name="T">The type of the object to be compared</typeparam>
		/// <param name="expected">The object that is expected not to be in the collection</param>
		/// <param name="collection">The collection to be inspected</param>
		/// <param name="comparer">The comparer used to equate objects in the collection with the expected object</param>
		/// <exception cref="DoesNotContainException">Thrown when the object is present inside the container</exception>
		public static IEnumerable<T> ShouldNotContain<T>(this IEnumerable<T> collection,
		                                                 T expected,
		                                                 IEqualityComparer<T> comparer)
		{
			Assert.DoesNotContain(expected, collection, comparer);
			return collection;
		}

		/// <summary>
		/// Verifies that the items of a collection are all equal to the items of another collection, and listed in the same order.
		/// </summary>
		/// <typeparam name="T">The type of the object to be compared</typeparam>
		/// <param name="actualCollection">The actual collection</param>
		/// <param name="expectedValues">The expected values</param>
		public static void ShouldBeEqualTo<T>(this IEnumerable<T> actualCollection, params T[] expectedValues)
		{
			Assert.Equal(expectedValues, actualCollection, new CollectionEqualityComparer<T>());
		}

		/// <summary>
		/// Verifies that the items of a collection are all equal to the items of another collection, and listed in the same order.
		/// </summary>
		/// <param name="actualCollection">The actual collection</param>
		/// <param name="expectedValues">The expected values</param>
		public static void ShouldBeEqualTo(this IEnumerable actualCollection, params object[] expectedValues)
		{
			Assert.Equal(expectedValues, actualCollection, new CollectionEqualityComparer());
		}

		/// <summary>
		/// Verifies that the items of a collection are all equal to the items of another collection, and are listed in the same order.
		/// </summary>
		/// <typeparam name="T">The type of the object to be compared</typeparam>
		/// <param name="expectedCollection">The expected collection</param>
		/// <param name="actualCollection">The actual collection</param>
		public static IEnumerable<T> ShouldBeEqualTo<T>(this IEnumerable<T> expectedCollection, IEnumerable<T> actualCollection)
		{
			Assert.Equal(expectedCollection, actualCollection, new CollectionEqualityComparer<T>());
			return expectedCollection;
		}

		/// <summary>
		/// Verifies that the items of a collection are not all equal to the items of another collection, and are not listed in the same order.
		/// </summary>
		/// <typeparam name="T">The type of the object to be compared</typeparam>
		/// <param name="collection">The expected collection</param>
		/// <param name="actualCollection">The actual collection</param>
		public static IEnumerable<T> ShouldNotBeEqualTo<T>(this IEnumerable<T> collection, IEnumerable<T> actualCollection)
		{
			Assert.NotEqual(collection, actualCollection, new CollectionEqualityComparer<T>());
			return collection;
		}

		/// <summary>
		/// Verifies that the items of a collection are all equal to the items of another collection, but they do not have be listed in the same order.
		/// </summary>
		/// <typeparam name="T">The type of the object to be compared</typeparam>
		/// <param name="collection">The expected collection</param>
		/// <param name="actualCollection">The actual collection</param>
		public static IEnumerable<T> ShouldHaveEquivalenceWith<T>(this IEnumerable<T> collection, IEnumerable<T> actualCollection)
			where T : IComparable<T>
		{
			Assert.Equal(collection, actualCollection, new CollectionEquivalenceComparer<T>());
			return collection;
		}

		/// <summary>
		/// Verifies that the items of a collection are not all equal to the items of another collection.
		/// </summary>
		/// <typeparam name="T">The type of the object to be compared</typeparam>
		/// <param name="collection">The expected collection</param>
		/// <param name="actualCollection">The actual collection</param>
		public static IEnumerable<T> ShouldNotHaveEquivalenceWith<T>(this IEnumerable<T> collection, IEnumerable<T> actualCollection)
			where T : IComparable<T>
		{
			Assert.NotEqual(collection, actualCollection, new CollectionEquivalenceComparer<T>());
			return collection;
		}
	}
}
