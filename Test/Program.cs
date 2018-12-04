using System;
using studious_doodle;

namespace Test
{
	class Program
	{
		static void Main(string[] args)
		{
			var json = new Lexer("{ \"foo\" : \"bar\", \"faz\" : 1337, \"unicode test\" : \"\\u00bb \\u00b5\", \"number test\" : 1.5e10 }");
			foreach (var token in json.Tokens)
			{
				Console.WriteLine(string.Format($"{token.Type, 14}{(!(token.Value is null) ? (" | " + token.Value) : "")}"));
			}

			Console.ReadLine();
		}
	}
}
