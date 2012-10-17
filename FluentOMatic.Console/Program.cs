using FluentOMatic.Emission;
using FluentOMatic.States;
using NDesk.Options;
using System.IO;
using System.Linq;

namespace FluentOMatic.Console
{
	class Program
	{
		static void Main(string[] args)
		{
			string inputFile = null;
			string outputFile = null;
			string namespaceName = null;
			bool help = false;

			var options = new OptionSet
			{
				{ "i|input=", v => inputFile = v },
				{ "o|output=", v => outputFile = v },
 				{ "ns|namespace=", v => namespaceName = v },
				{ "h|?|help", v => help = true },
			};

			options.Parse(args);
			if (help)
			{
				options.WriteOptionDescriptions(System.Console.Out);
				return;
			}

			var parser = new Parser(inputFile != null ? new Scanner(inputFile) : new Scanner(System.Console.OpenStandardInput()));
			parser.Parse();

			var graphBuilder = new StateGraphBuilder();
			var states = graphBuilder.BuildGraph(parser.Syntax);

			var output = outputFile != null ? File.CreateText(outputFile) : System.Console.Out;

			var generator = new CodeGenerator();
			generator.GenerateCode(states.First(), output, namespaceName ?? parser.Syntax.Name);

			output.Flush();
			output.Close();
		}
	}
}
