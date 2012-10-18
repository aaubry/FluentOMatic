//    Copyright 2012 Antoine Aubry
//    
//    This file is part of FluentOMatic.
//
//    FluentOMatic is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    FluentOMatic is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with FluentOMatic.  If not, see <http://www.gnu.org/licenses/>.

using FluentOMatic.Emission;
using FluentOMatic.States;
using NDesk.Options;
using System.IO;
using System.Linq;

namespace FluentOMatic.Console
{
	class Program
	{
		static int Main(string[] args)
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
				var notice = @"
					This program is free software: you can redistribute it and/or modify
					it under the terms of the GNU General Public License as published by
					the Free Software Foundation, either version 3 of the License, or
					(at your option) any later version.

					This program is distributed in the hope that it will be useful,
					but WITHOUT ANY WARRANTY; without even the implied warranty of
					MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
					GNU General Public License for more details.

					You should have received a copy of the GNU General Public License
					along with this program.  If not, see <http://www.gnu.org/licenses/>.


					USAGE:
				";

				System.Console.WriteLine(notice.Replace("\t", ""));

				options.WriteOptionDescriptions(System.Console.Out);
				return 2;
			}

			var parser = new Parser(inputFile != null ? new Scanner(inputFile) : new Scanner(System.Console.OpenStandardInput()));

			var previousConsoleColor = System.Console.ForegroundColor;
			System.Console.ForegroundColor = System.ConsoleColor.Red;
			parser.Parse();
			System.Console.ForegroundColor = previousConsoleColor;

			if (parser.errors.count != 0)
			{
				return 1;
			}

			var graphBuilder = new StateGraphBuilder();
			var states = graphBuilder.BuildGraph(parser.Syntax);

			var output = outputFile != null ? File.CreateText(outputFile) : System.Console.Out;

			var generator = new CodeGenerator();
			generator.GenerateCode(states.First(), output, namespaceName ?? parser.Syntax.Name, parser.Syntax.Usings);

			output.Flush();
			output.Close();

			return 0;
		}
	}
}
