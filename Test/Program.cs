using studious_doodle;

using System;
using System.Collections.Generic;

namespace Test
{
	class Program
	{
		static void Main(string[] args)
		{
			var json = JSON.ParseFile("test.json");

			json.nested.nested.shit = new object[]{
				"test",
				123456,
				1e-3,
				new int[]{ 1, 3, 3, 7 },
				new Dictionary<string,object>()
				{
					{ "key1", "value1" },
					{ "key2", 10 }
				}
			};
			json.null_test = null;
			//Console.WriteLine(json.array[1].key);

			var parms = new StringifyParameters()
			{
				UseTabs = false,
				SpacesPerIndent = 2,
				Minify = false
			};

			Console.WriteLine(JSON.Stringify(json, parms));

			Console.ReadLine();
		}
	}
}
