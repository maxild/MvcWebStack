using System;
using System.Xml;
using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Web.Mvc.UnitTests.Html5.AssertionExtensions
{
	public abstract class FragmentVerifier<T> where T : FragmentVerifier<T>
	{
		private readonly string _xhtml;

		protected FragmentVerifier(string xhtml)
		{
			_xhtml = xhtml;
		}

// ReSharper disable once InconsistentNaming
		protected T self { get { return (T) this; }}

		public T HasName(string expectedTagName)
		{
			Assert.NotNull(CurrentElement);
			Assert.Equal(expectedTagName, CurrentElement.Name);
			return self;
		}

		public T HasAttribute(string attributeName, string expectedValue)
		{
			Assert.NotNull(CurrentElement);
			string actualValue = CurrentElement.GetAttribute(attributeName);
			Assert.Equal(expectedValue, actualValue, StringComparer.Ordinal);
			return self;
		}

		public T DoesntHaveAttribute(string attributeName)
		{
			Assert.NotNull(CurrentElement);
			Assert.False(CurrentElement.HasAttribute(attributeName));
			return self;
		}

		public T HasInnerText(string content)
		{
			Assert.NotNull(CurrentElement);
			CurrentElement.InnerText.ShouldEqual(content);
			return self;
		}

		protected abstract XmlElement CurrentElement { get; }

		public override string ToString()
		{
			return _xhtml;
		}
	}
}