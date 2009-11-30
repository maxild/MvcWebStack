using System;

namespace Maxfire.Web.Mvc
{
	public class TextValuePair : ITextValuePair, IEquatable<TextValuePair>
	{
		public TextValuePair(string text, string value)
		{
			Text = text;
			Value = value;
		}
		public string Text { get; private set; }
		public string Value { get; private set; }
		
		public bool Equals(TextValuePair other)
		{
			if (other == null)
				return false;

			return GetType() == other.GetType() &&
			       Text.Equals(other.Text, StringComparison.CurrentCulture) &&
			       Value.Equals(other.Value, StringComparison.Ordinal);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as TextValuePair);
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
}