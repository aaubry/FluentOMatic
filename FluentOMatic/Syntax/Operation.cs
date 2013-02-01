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

using System.Collections.Generic;

namespace FluentOMatic.Syntax
{
	public class Operation
	{
		public string Name { get; set; }
		public Multiplicity Multiplicity { get; set; }
		public ParameterList Parameters { get; private set; }
		public OperationGroupList OperationGroups { get; private set; }

		public bool IsRequired { get { return Multiplicity == Syntax.Multiplicity.One || Multiplicity == Syntax.Multiplicity.OneOrMany; } }
		public bool IsMultiple { get { return Multiplicity == Syntax.Multiplicity.ZeroOrMany || Multiplicity == Syntax.Multiplicity.OneOrMany; } }

		public Operation()
		{
			Parameters = new ParameterList();
			OperationGroups = new OperationGroupList();
		}

		public override string ToString()
		{
			return string.Format("{0} [{1}]", Name, Multiplicity);
		}
	}
}