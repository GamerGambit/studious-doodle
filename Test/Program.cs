using studious_doodle;

using System;

namespace Test
{
	class Program
	{
		static void Main(string[] args)
		{
			var json = JSON.ParseString(@"
{
	""foo"" : ""bar"",
	""faz"" : 1337,
	""unicode_test"" : ""\u00bb \u00b5"",
	""number_test"" : 1.5e10,
	""Booln"" : false,
	""nested"" : {
		""value"" : 7331,
		""nested"" : {
			""shit"" : ""works""
		}
	},
	""array"" : [
		11333377,
		{
			""key"" : ""value""
		},
		[
			""nested""
		]
	]
}");
			json.nested.nested.shit = "changed";
			Console.WriteLine(json.array[1].key);
			Console.ReadLine();
		}
	}
}
