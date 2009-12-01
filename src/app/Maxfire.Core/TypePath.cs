using Maxfire.Core.Extensions;

namespace Maxfire.Core
{
	public static class TypePath
	{
		const char NAMESPACE_SEPARATOR_CHAR = '.';

		public static string GetName(string path)
		{
			if (path != null)
			{
				int idx = path.Length;
				while (--idx >= 0)
				{
					char ch = path[idx];
					if (ch == NAMESPACE_SEPARATOR_CHAR)
					{
						return path.Substring(idx + 1, (path.Length - idx) - 1);
					}
				}
			}
			return path;
		}

		public static string GetNamespacePath(string path)
		{
			if (path != null)
			{
				string name = GetName(path);
				int length = path.Length - name.Length - 1;
				return ((length > 0) ? path.Substring(0, length) : string.Empty);
			}
			return path;
		}

		public static string Combine(string path1, string path2)
		{
			if (path1.IsEmpty())
			{
				return path2;
			}
			if (path2.IsEmpty())
			{
				return path1;
			}
			char ch = path1[path1.Length - 1];
			if (ch != NAMESPACE_SEPARATOR_CHAR)
			{
				return path1 + NAMESPACE_SEPARATOR_CHAR + path2;
			}
			return path1 + path2;
		}
	}
}