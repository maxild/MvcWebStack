namespace Maxfire.Web.Mvc.Html5.Elements
{
	public class Input : InputElement<Input> 
	{
		public Input(string type, string name, IModelMetadataAccessor accessor) 
			: base(type, name, accessor)
		{
			if (accessor != null)
			{
				SetValueAttr(accessor.GetModelValueAsString(name));
			}
		}

		protected override void BindValue(object value)
		{
			SetValueAttr(value);
		}

		public override Input Value(object value)
		{
			BindExplicitValue(value); // we cannot call SetValueAttr, because we need explicit value flag to be set
			return this;
		}
	}
}