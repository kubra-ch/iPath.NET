using System.Text;
using System.Web;
using System.Text.RegularExpressions;

namespace iPath.Application.Services;

public class StringConversionService
{
	public static string DataXmlConversion(string data)
	{
		var c1 =  Encoding.UTF8.GetString(Encoding.GetEncoding("ISO-8859-1").GetBytes(data));

		// var c1 = Encoding.GetEncoding("ISO-8859-1").GetString(Encoding.UTF8.GetBytes(data));

		return c1;
	}


	public static string StringToHtml(string text)
	{
		text ??= "";
		if (ContainsHtml(text))
		{
			return text;
		}
		else
		{
			string html = text;

			html = html.Replace("\n", "<br />");
			html = html.Replace("\r", "<br />");

			// convert http links to <a> tag
			var mathces = Regex.Matches(html, "https?:\\/\\/(www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b([-a-zA-Z0-9()@:%_\\+.~#?&//=]*)");

			foreach( Match m in mathces)
			{
				if(m.Success)
				{
					html = html.Replace(m.Groups[0].Value, "<a target=\"_blank\" href=\"" + m.Groups[0].Value + "\">" + m.Groups[0].Value + "</a>");
				}
			}

			return html;
		}
	}

	public static bool ContainsHtml(string text)
	{
		if (string.IsNullOrEmpty(text)) return false;
		var m = Regex.Match(text, "<[^>]+>");
		return m.Success;
	}
}
