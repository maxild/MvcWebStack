using System;

namespace Maxfire.Core
{
	public class TextValuePair<TValue> : ITextValuePair<TValue>, IEquatable<TextValuePair<TValue>>
	{
		public TextValuePair(string text, TValue value)
		{
			Text = text;
			Value = value;
		}

		public string Text { get; private set; }

		public TValue Value { get; private set; }
		
		public bool Equals(TextValuePair<TValue> other)
		{
			if (other == null)
				return false;

			return GetType() == other.GetType() &&
			       Text.Equals(other.Text, StringComparison.CurrentCulture) &&
			       Value.Equals(other.Value);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as TextValuePair<TValue>);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = Text.GetHashCode();
				hashCode = (hashCode * 397) ^ Value.GetHashCode();
				return hashCode;
			}
		}
	}

	public class TextValuePair : TextValuePair<string>, ITextValuePair
	{
		public TextValuePair(string text, string value) : base(text, value)
		{
		}
	}
}