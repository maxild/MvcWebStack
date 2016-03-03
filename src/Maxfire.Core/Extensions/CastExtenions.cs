namespace Maxfire.Core.Extensions
{
	public static class CastsExtensions
	{
		public static T CastTo<T>(this object o)
		{
			return (T)o;
		}

		public static T TryCastTo<T>(this object o) where T : class
		{
			return o as T;
		}
	}
}