namespace studious_doodle
{
	public static class JSON
	{
		public static dynamic ParseString(string source)
		{
			var parser = new Parser(source);
			return parser.document;
		}
	}
}
