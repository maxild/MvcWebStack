using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Web.Mvc.UnitTests
{
	public class DateModelBinderTester
	{
		[Fact]
		public void CanHandleSingleField()
		{
			var valueProvider = new SimpleValueProvider { { "", "3/6/1970" } };

			var bindingContext = new ModelBindingContext
			{
				ModelName = "",
				FallbackToEmptyPrefix = true,
				ValueProvider = valueProvider
			};

			var dateAndTimeModelBinder = new DateModelBinder(new CultureInfo("da-DK"));

			DateTime? result = (DateTime?)dateAndTimeModelBinder.BindModel(null, bindingContext);
			Assert.Equal(new DateTime(1970, 6, 3), result);
		}

		[Fact]
		public void CanHandleSingleNamedField()
		{
			var valueProvider = new SimpleValueProvider { { "birthday", "3/6/1970" } };

			var bindingContext = new ModelBindingContext
			{
				ModelName = "birthday",
				ValueProvider = valueProvider
			};

			var dateAndTimeModelBinder = new DateModelBinder(new CultureInfo("da-DK"));

			DateTime? result = (DateTime?)dateAndTimeModelBinder.BindModel(null, bindingContext);
			Assert.Equal(new DateTime(1970, 6, 3), result);
		}

		[Fact]
		public void CanHandleMultipleFields()
		{
			var valueProvider = new SimpleValueProvider
			{ 
				{ "day", "12" },
				{ "month", "2" },
				{ "year", "1964" }
			};

			var bindingContext = new ModelBindingContext
			{
				FallbackToEmptyPrefix = true,
				ModelName = "",
				ValueProvider = valueProvider
			};

			var dateAndTimeModelBinder = new DateModelBinder
			{
				Day = "day",
				Month = "month",
				Year = "year"
			};

			var result = (DateTime?)dateAndTimeModelBinder.BindModel(null, bindingContext);
			Assert.Equal(new DateTime(1964, 2, 12), result);
		}

		[Fact]
		public void CanHandleNamedMultipleFields()
		{
			var valueProvider = new SimpleValueProvider
			{ 
				{ "birthday.day", "12" },
				{ "birthday.month", "2" },
				{ "birthday.year", "1964" }
			};

			var bindingContext = new ModelBindingContext
			{
				ModelName = "birthday",
				ValueProvider = valueProvider
			};

			var dateAndTimeModelBinder = new DateModelBinder
			{
				Day = "day",
				Month = "month",
				Year = "year"
			};

			var result = (DateTime?)dateAndTimeModelBinder.BindModel(null, bindingContext);
			Assert.Equal(new DateTime(1964, 2, 12), result);
		}

		[Fact]
		public void CanSerialize()
		{
			var serializer = new DateModelBinder();

			var values = serializer.GetValues(new DateTime(1970, 6, 3), string.Empty);

			values.Count.ShouldEqual(3);
			values["Day"].ShouldEqual("3");
			values["Month"].ShouldEqual("6");
			values["Year"].ShouldEqual("1970");
		}

		[Fact]
		public void CanSerializeWithPrefix()
		{
			var serializer = new DateModelBinder
			{
				Day = "day",
				Month = "month",
				Year = "year"
			};

			var values = serializer.GetValues(new DateTime(1970, 6, 3), "foedselsdato");

			values.Count.ShouldEqual(3);
			values["foedselsdato.day"].ShouldEqual("3");
			values["foedselsdato.month"].ShouldEqual("6");
			values["foedselsdato.year"].ShouldEqual("1970");
		}

		class SimpleValueProvider : Dictionary<string, object>, IValueProvider
		{
			private readonly CultureInfo _culture;

			public SimpleValueProvider()
				: this(null)
			{
			}

			public SimpleValueProvider(CultureInfo culture)
				: base(StringComparer.OrdinalIgnoreCase)
			{
				_culture = culture ?? CultureInfo.InvariantCulture;
			}

			// copied from ValueProviderUtil
			public bool ContainsPrefix(string prefix)
			{
				foreach (string key in Keys)
				{
					if (key != null)
					{
						if (prefix.Length == 0)
						{
							return true; // shortcut - non-null key matches empty prefix
						}

						if (key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
						{
							if (key.Length == prefix.Length)
							{
								return true; // exact match
							}

							switch (key[prefix.Length])
							{
								case '.': // known separator characters
								case '[':
									return true;
							}
						}
					}
				}

				return false; // nothing found
			}

			public ValueProviderResult GetValue(string key)
			{
				object rawValue;
				if (TryGetValue(key, out rawValue))
				{
					return new ValueProviderResult(rawValue, Convert.ToString(rawValue, _culture), _culture);
				}
				// value not found
				return null;
			}

			public IEnumerable<string> GetKeys()
			{
				return Keys;
			}
		}

	}
}