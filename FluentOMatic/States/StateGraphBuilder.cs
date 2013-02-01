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

using FluentOMatic.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace FluentOMatic.States
{
	public class StateGraphBuilder
	{
		public ICollection<State> BuildGraph(FluentSyntax syntax)
		{
			var allStates = new List<State>();
			BuildGraph(syntax.Name, syntax.OperationGroups, allStates);
			return allStates;
		}

		private State BuildGraph(string name, OperationGroupList operationGroups, ICollection<State> allStates)
		{
			var entryState = new State(name, true, Enumerable.Empty<Parameter>());
			allStates.Add(entryState);

			var previousStates = new List<State> { entryState };
			foreach (var operation in operations)
			{
				BuildGraph(previousStates, operation, allStates);
			}
			return entryState;
		}

		private void BuildGraph(IList<State> previousStates, Operation operation, ICollection<State> allStates)
		{
			var currentState = new State(operation.Name, false, operation.Parameters);
			allStates.Add(currentState);

			foreach (var previousState in previousStates)
			{
				previousState.NextStates.Add(currentState);
			}

			switch (operation.Multiplicity)
			{
				case Multiplicity.One:
					previousStates.Clear();
					previousStates.Add(currentState);
					break;

				case Multiplicity.ZeroOrOne:
					previousStates.Add(currentState);
					currentState.IsOptional = true;
					break;

				case Multiplicity.OneOrMany:
					previousStates.Clear();
					previousStates.Add(currentState);

					currentState.NextStates.Add(currentState);
					break;

				case Multiplicity.ZeroOrMany:
					previousStates.Add(currentState);
					currentState.IsOptional = true;

					currentState.NextStates.Add(currentState);
					break;
			}

			if (operation.OperationGroups.Any())
			{
				currentState.InnerState = BuildGraph(operation.Name + "Inner", operation.Operations, allStates);
			}
		}
	}
}
