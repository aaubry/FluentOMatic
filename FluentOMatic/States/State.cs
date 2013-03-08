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
	public class State
	{
		public string Name { get; private set; }
		public IEnumerable<Parameter> Parameters { get; private set; }
		public ICollection<State> NextStates { get; private set; }

		public State InnerState { get; set; }
		public bool IsOptional { get; set; }
		public bool IsRoot { get; private set; }

		public bool IsTerminal
		{
			get
			{
				return NextStates.All(s => s.IsOptional || ReferenceEquals(s, this));
			}
		}

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
