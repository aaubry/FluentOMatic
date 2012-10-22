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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace FluentOMatic.Tests.Integration
{
	public class StateGraphTests
	{
		private void AssertAllStatesCanBeReached(IEnumerable<State> states)
		{
			var allStates = new HashSet<State>(states);

			var visitedStates = new HashSet<State>();
			TraverseStateGraph(states.First(), visitedStates);

			var unreachableStates = allStates.Except(visitedStates).ToList();

			if (unreachableStates.Count != 0)
			{
				Console.WriteLine("Unreachable states:");
				foreach (var unreachableState in unreachableStates)
				{
					Console.WriteLine(unreachableState.Name);
				}
			}

			Assert.Equal(allStates.Count, visitedStates.Count);
		}

		private void TraverseStateGraph(State currentState, HashSet<State> visitedStates)
		{
			if (visitedStates.Add(currentState))
			{
				foreach (var nextState in currentState.NextStates)
				{
					TraverseStateGraph(nextState, visitedStates);
				}
				if (currentState.InnerState != null)
				{
					TraverseStateGraph(currentState.InnerState, visitedStates);
				}
			}
		}

		[Fact]
		public void OneOrMany_followed_by_another_method_can_reach_that_method()
		{
			var syntax = @"
				syntax s
				.First()+
				.Second()
			";

			var parser = new Parser(new Scanner(new MemoryStream(Encoding.UTF8.GetBytes(syntax))));
			parser.Parse();

			Assert.Equal(0, parser.errors.count);

			var graphBuilder = new StateGraphBuilder();
			var states = graphBuilder.BuildGraph(parser.Syntax);

			Assert.Equal(3, states.Count);

			AssertAllStatesCanBeReached(states);
		}
	}
}