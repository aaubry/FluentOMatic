using System.Collections.Generic;

namespace FluentOMatic.Syntax
{
	public class Operation
	{
		public string Name { get; set; }
		public Multiplicity Multiplicity { get; set; }
		public ParameterList Parameters { get; private set; }
		public OperationList Operations { get; private set; }

		public bool IsRequired { get { return Multiplicity == Syntax.Multiplicity.One || Multiplicity == Syntax.Multiplicity.OneOrMany; } }
		public bool IsMultiple { get { return Multiplicity == Syntax.Multiplicity.ZeroOrMany || Multiplicity == Syntax.Multiplicity.OneOrMany; } }

		public Operation()
		{
			Parameters = new ParameterList();
			Operations = new OperationList();
		}

		public override string ToString()
		{
			return string.Format("{0} [{1}]", Name, Multiplicity);
		}
	}
}