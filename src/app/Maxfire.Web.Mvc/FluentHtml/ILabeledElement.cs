namespace Maxfire.Web.Mvc.FluentHtml
{
	public interface ILabeledElement 
	{
		/// <summary>
		/// The text for the label rendered before the element.
		/// </summary>
		string LabelBeforeText { get; set; }

		/// <summary>
		/// The text for the label rendered after the element.
		/// </summary>
		string LabelAfterText { get; set; }

		/// <summary>
		/// The class for labels rendered before or after the element.
		/// </summary>
		string LabelClass { get; set; }
	}
}