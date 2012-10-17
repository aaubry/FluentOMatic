using System.Collections.Generic;

namespace FluentOMatic.Syntax
{
	public class Parameter
	{
		public string Name { get; set; }

		private string _type;

		public string Type
		{
			get
			{
				return _type;
			}
			set
			{
				string typeName;
				_type = _builtInTypes.TryGetValue(value, out typeName) ? typeName : value;
			}
		}

		private static Dictionary<string, string> _builtInTypes = new Dictionary<string, string>
		{
			{ "bool", "Boolean" },
			{ "byte", "Byte" },
			{ "sbyte", "SByte" },
			{ "char", "Char" },
			{ "decimal", "Decimal" },
			{ "double", "Double" },
			{ "float", "Single" },
			{ "int", "Int32" },
			{ "uint", "UInt32" },
			{ "long", "Int64" },
			{ "ulong", "UInt64" },
			{ "object", "Object" },
			{ "short", "Int16" },
			{ "ushort", "UInt16" },
			{ "string", "String" },
		};

		public override string ToString()
		{
			return string.Format("{0}:{1}", Name, Type);
		}
	}
}