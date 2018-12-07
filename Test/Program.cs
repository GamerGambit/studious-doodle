using studious_doodle;

using System;

namespace Test
{
	class Program
	{
		static void Main(string[] args)
		{
			var json = JSON.ParseFile("test.json");

			//json.nested.nested.shit = "changed";
			Console.WriteLine(json.array[1].key);

			Console.ReadLine();
		}
	}
}
