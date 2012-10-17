﻿using FluentOMatic.Syntax;
using System.Collections.Generic;

namespace FluentOMatic.States
{
	public class State
	{
		public string Name { get; private set; }
		public IEnumerable<Parameter> Parameters { get; private set; }
		public ICollection<State> NextStates { get; private set; }

		public State InnerState { get; set; }
		public bool IsOptional { get; set; }
		public bool IsRoot { get; private set; }

		public State(string name, bool isRoot, IEnumerable<Parameter> parameters)
		{
			Name = name;
			IsRoot = isRoot;
			Parameters = parameters;
			NextStates = new List<State>();
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
