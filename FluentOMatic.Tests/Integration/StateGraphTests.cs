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

using FluentOMatic.States;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Xunit;

namespace FluentOMatic.Tests.Integration
{
	public class StateGraphTests
	{
		private ICollection<State> ParseSyntaxToGraph(string syntax)
		{
			var parser = new Parser(new Scanner(new MemoryStream(Encoding.UTF8.GetBytes(syntax))));
			parser.Parse();

			Assert.Equal(0, parser.errors.count);

			var graphBuilder = new StateGraphBuilder();
			return graphBuilder.BuildGraph(parser.Syntax);
		}

		[Fact]
		public void OneOrMany_followed_by_another_method_can_reach_that_method()
		{
			var states = ParseSyntaxToGraph(@"
				syntax s
				.First()+
				.Second()
			");

			Assert.Equal(3, states.Count);
			StateGraphAssert.AllStatesCanBeReached(states);
		}

		[Fact]
		public void Last_state_is_terminal_when_OneOrMany()
		{
			var states = ParseSyntaxToGraph(@"
				syntax s
				.First()+
			");

			Assert.True(states.Last().IsTerminal);
		}

		[Fact]
		public void Alternatives_are_present()
		{
			var states = ParseSyntaxToGraph(@"
				syntax s
				(
					.Choice1()
				|
					.Choice2()
				)
			");

			//var entryState = states.First();
			//entryState.n

			Assert.True(states.Last().IsTerminal);
		}

		[Fact]
		public void Alternatives_are_terminal()
		{
			var states = ParseSyntaxToGraph(@"
				syntax s
				(
					.Choice1()
				|
					.Choice2()
				)
			");

			Assert.True(states.Last().IsTerminal);
		}
	}
}