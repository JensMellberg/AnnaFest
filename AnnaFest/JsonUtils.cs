using Newtonsoft.Json;
using System.Text;

namespace AnnaFest
{
	public static class JsonUtils
	{
		public static char[] AllowedCharacters = new char[]
		{
			',',
			'.',
			' ',
			'!',
			'#',
			'%',
			'&',
			'/',
			'(',
			')',
			'=',
			'?',
			'|',
			'/',
			'§',
			'<',
			'>',
			'´',
			'`',
			'^',
			'*',
			'~',
			':',
			';'
		};
		public static string GetJavascriptText(string varName, object obj)
		{
			return $"var {varName} = JSON.parse('{SerializeForJavascript(obj)}')";
		}

		public static string SerializeForJavascript(object obj)
		{
			return JsonConvert.SerializeObject(obj).Replace("\\", "").Replace("'", "\\'");
		}

		public static string RemoveAllWeirdness(string s) {
			if (string.IsNullOrEmpty(s))
			{
				return s;
			}

			var sb = new StringBuilder();
			foreach (var c in s)
			{
				if (c >= '0' && c <= '9')
				{
					sb.Append(c);
				}
				else if (c >= 'a' && c <= 'ö')
				{
					sb.Append(c);
				}
				else if (c >= 'A' && c <= 'Ö')
				{
					sb.Append(c);
				}
                else if (c >= 200)
                {
                    sb.Append(c);
                }
                else if (AllowedCharacters.Contains(c))
				{
					sb.Append(c);
				}
			}

			return sb.ToString();
		}
    }
}
