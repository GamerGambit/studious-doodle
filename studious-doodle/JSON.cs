using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace studious_doodle
{
	public struct StringifyParameters
	{
		public static StringifyParameters Default = new StringifyParameters()
		{
			UseTabs = true,
			SpacesPerIndent = 0,
			Minify = false
		};

		public bool UseTabs;
		public int SpacesPerIndent;
		public bool Minify;
	}

	public static class JSON
	{
		public static dynamic ParseString(string source)
		{
			var parser = new Parser(source);
			return parser.document;
		}

		public static dynamic ParseFile(string path)
		{
			var contents = File.ReadAllText(path);
			return ParseString(contents);
		}

		public static string Stringify(dynamic obj)
		{
			return Stringify(obj, StringifyParameters.Default);
		}

		public static string Stringify(dynamic obj, StringifyParameters stringifyParameters)
		{
			return Stringify(obj, stringifyParameters, 0);
		}

		private static string Stringify(dynamic value, StringifyParameters stringifyParameters, int indent, bool ignoreInitialPadding = false)
		{
			string leftpadding()
			{
				if (stringifyParameters.Minify)
					return string.Empty;

				if (stringifyParameters.UseTabs)
					return new string('\t', indent);

				return new string(' ', indent * stringifyParameters.SpacesPerIndent);
			}
			string newline()
			{
				return stringifyParameters.Minify ? string.Empty : "\n";
			}

			var sb = new StringBuilder();

			if (!ignoreInitialPadding)
				sb.Append(leftpadding());

			if (value is null)
			{
				sb.Append("null");
			}
			else if (value is IDictionary<string, object>)
			{
				sb.Append('{' + newline());

				bool trailingNewLine = false;
				bool trailingComma = false;
				foreach (var pair in value)
				{
					if (trailingComma)
					{
						sb.Append(',' + newline());
					}

					sb.Append(Stringify(pair.Key, stringifyParameters, indent + 1));
					sb.AppendFormat(":{0}", stringifyParameters.Minify ? string.Empty : " ");
					sb.Append(Stringify(pair.Value, stringifyParameters, indent + 1, true));

					trailingComma = true;
					trailingNewLine = !stringifyParameters.Minify;
				}

				if (trailingNewLine)
				{
					sb.Append(newline());
				}

				sb.Append(leftpadding() + '}');
			}
			else if (value.GetType().IsArray || value is IList<object>)
			{
				sb.Append('[' + newline());

				bool trailingNewLine = false;
				bool trailingComma = false;
				foreach (var element in value)
				{
					if (trailingComma)
					{
						sb.Append(',' + newline());
					}

					sb.Append(Stringify(element, stringifyParameters, indent + 1));

					trailingComma = true;
					trailingNewLine = !stringifyParameters.Minify;
				}

				if (trailingNewLine)
				{
					sb.Append(newline());
				}

				sb.Append(leftpadding() + ']');
			}
			else if (double.TryParse(value.ToString(), out double res))
			{
				sb.Append(res);
			}
			else if (value is string)
			{
				sb.AppendFormat(@"""{0}""", value);
			}
			else if (value is bool b)
			{
				sb.Append(b.ToString().ToLower());
			}
			else
			{
				var type = value.GetType();
				throw new Exception("Invalid JSON value type");
			}

			return sb.ToString();
		}
	}
}
