using FluentOMatic.Emission;
using FluentOMatic.States;
using System.IO;
using System.Linq;

namespace FluentOMatic
{
	class Program
	{
		static void Main(string[] args)
		{
			var parser = new Parser(new Scanner("test.txt"));
			parser.Parse();

			var graphBuilder = new StateGraphBuilder();
			var states = graphBuilder.BuildGraph(parser.Syntax);

			using (var output = File.CreateText(@"\Work\FluentOMatic\ConsoleApplication1\output.cs"))
			{
				var generator = new CodeGenerator();
				generator.GenerateCode(states.First(), output, "Osd.Syntax");
			}
		}
	}
}
