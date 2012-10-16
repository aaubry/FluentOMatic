﻿using FluentOMatic.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace FluentOMatic.States
{
	public class StateGraphBuilder
	{
		public ICollection<State> BuildGraph(FluentSyntax syntax)
		{
			var allStates = new List<State>();
			BuildGraph(syntax.Name, syntax.Operations, allStates);
			return allStates;
		}

		private State BuildGraph(string name, OperationList operations, ICollection<State> allStates)
		{
			var entryState = new State(name, Enumerable.Empty<Parameter>());
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
			var currentState = new State(operation.Name, operation.Parameters);
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
					currentState.NextStates.Add(currentState);
					break;

				case Multiplicity.ZeroOrMany:
					previousStates.Add(currentState);
					currentState.NextStates.Add(currentState);
					currentState.IsOptional = true;
					break;
			}

			if (operation.Operations.Any())
			{
				currentState.InnerState = BuildGraph(operation.Name + "Inner", operation.Operations, allStates);
			}
		}
	}
}
