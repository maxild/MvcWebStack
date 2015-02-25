namespace Maxfire.Core
{
	public class SortOrder : ConvertibleEnumeration<SortOrder>
	{
		public static readonly SortOrder Ascending = new SortOrder(1, "Ascending", "Ascending sort order");
		public static readonly SortOrder Descending = new SortOrder(2, "Descending", "Descending sort order");

		private SortOrder(int value, string name, string text)
			: base(value, name, text)
		{
		}
	}
}