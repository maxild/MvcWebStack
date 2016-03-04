using System.Text;

namespace Maxfire.Web.Mvc.Html
{
	public static class Html401IdUtil
	{
		public static string CreateSanitizedId(string originalId, string invalidCharacterReplacement = "_")
		{
			if (string.IsNullOrEmpty(originalId))
			{
				return null;
			}

			char firstChar = originalId[0];
			if (!IsLetter(firstChar))
			{
				return null;
			}

			var sb = new StringBuilder(originalId.Length);
			sb.Append(firstChar);

			for (int i = 1; i < originalId.Length; i++)
			{
				char thisChar = originalId[i];
				if (IsValidIdCharacter(thisChar))
				{
					sb.Append(thisChar);
				}
				else
				{
					sb.Append(invalidCharacterReplacement);
				}
			}

			return sb.ToString();
		}

		private static bool IsLetter(char c)
		{
			return (('A' <= c && c <= 'Z') || ('a' <= c && c <= 'z'));
		}

		private static bool IsValidIdCharacter(char c)
		{
			return (IsLetter(c) || IsDigit(c) || IsAllowableSpecialCharacter(c));
		}

		private static bool IsAllowableSpecialCharacter(char c)
		{
			switch (c)
			{
				case '-': case '_': case ':':
					// note that we're specifically excluding the '.' character
					return true;
				default:
					return false;
			}
		}

		private static bool IsDigit(char c)
		{
			return ('0' <= c && c <= '9');
		}
	}
}
