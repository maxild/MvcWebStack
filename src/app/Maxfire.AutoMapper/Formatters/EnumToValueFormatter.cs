using System;

namespace Maxfire.AutoMapper.Web.Mvc.Formatters
{
	public class EnumToValueFormatter<TEnum> : ValueFormatter<TEnum>
	{
		protected override string FormatValueCore(TEnum value)
		{
			Type enumType = typeof(TEnum);
			if (!enumType.IsEnum)
			{
				throw new ArgumentException("The generic type argument must be an enum.");
			}

			return Convert.ToInt32(value).ToString();
		}
	}
}