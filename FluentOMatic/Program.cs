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

			//// ----
			//int index = 0;
			//var stateNames = states.ToDictionary(s => s, s => "S" + index++);
			//Console.WriteLine("digraph {");
			//foreach (var state in states)
			//{
			//	Console.WriteLine("  {0} [label=\"{1}\"];", stateNames[state], state.Name);

			//	foreach (var nextState in state.NextStates)
			//	{
			//		Console.WriteLine("  {0} -> {1};", stateNames[state], stateNames[nextState]);
			//	}
			//}
			//Console.WriteLine("}");
			//// ----

			using (var output = File.CreateText(@"\Work\FluentOMatic\ConsoleApplication1\output.cs"))
			{
				var generator = new CodeGenerator();
				generator.GenerateCode(states.First(), output);
			}
		}
	}
}
