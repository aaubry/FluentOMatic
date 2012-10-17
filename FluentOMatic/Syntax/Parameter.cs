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
//    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.

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