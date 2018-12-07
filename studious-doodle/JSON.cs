using System.IO;

namespace studious_doodle
{
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
	}
}
