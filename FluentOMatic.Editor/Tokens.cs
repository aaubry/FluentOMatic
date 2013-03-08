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

using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text.Classification;
using System.Collections.Generic;
using System.Linq;

namespace FluentOMatic.Editor
{
	partial class Editor
	{
		private readonly TokenTypes _tokenTypes;

		internal Editor(IClassificationTypeRegistryService registry)
		{
			_tokenTypes = new TokenTypes(registry);
		}

		private class TokenType
		{
			public int Id { get; private set; }
			public IClassificationType ClassificationType { get; private set; }

			public TokenType(int id, IClassificationType classificationType)
			{
				Id = id;
				ClassificationType = classificationType;
			}
		}

		private class TokenTypes
		{
			private readonly IClassificationType _defaultClassificationType;
			private readonly IDictionary<int, TokenType> _tokenTypesById;

			public TokenTypes(IClassificationTypeRegistryService registry)
			{
				_defaultClassificationType = registry.GetClassificationType(PredefinedClassificationTypeNames.ExcludedCode);

				_tokenTypesById = new[]
				{
					// Defined in syntax
					Dot = new TokenType(Parser._dot, registry.GetClassificationType(PredefinedClassificationTypeNames.Operator)),
					Identifier = new TokenType(Parser._ident, registry.GetClassificationType(PredefinedClassificationTypeNames.Identifier)),
					Number = new TokenType(Parser._number, registry.GetClassificationType(PredefinedClassificationTypeNames.Number)),
					OpenParen = new TokenType(Parser._openParen, registry.GetClassificationType(PredefinedClassificationTypeNames.Operator)),
					CloseParen = new TokenType(Parser._closeParen, registry.GetClassificationType(PredefinedClassificationTypeNames.Operator)),
					ZeroOrMany= new TokenType(Parser._zeroOrMany, registry.GetClassificationType(PredefinedClassificationTypeNames.Keyword)),
					ZeroOrOne= new TokenType(Parser._zeroOrOne, registry.GetClassificationType(PredefinedClassificationTypeNames.Keyword)),
					OneOrMany= new TokenType(Parser._oneOrMany, registry.GetClassificationType(PredefinedClassificationTypeNames.Keyword)),
					ParameterSeparator = new TokenType(Parser._parameterSep, registry.GetClassificationType(PredefinedClassificationTypeNames.Operator)),
					Syntax= new TokenType(Parser._syntax, registry.GetClassificationType(PredefinedClassificationTypeNames.Keyword)),
					String = new TokenType(Parser._string, registry.GetClassificationType(PredefinedClassificationTypeNames.SymbolReference)),
					Using = new TokenType(Parser._using, registry.GetClassificationType(PredefinedClassificationTypeNames.Keyword)),
					GenericArgumentListStart = new TokenType(Parser._genericArgListStart, registry.GetClassificationType(PredefinedClassificationTypeNames.Keyword)),
					GenericArgumentListEnd = new TokenType(Parser._genericArgListEnd, registry.GetClassificationType(PredefinedClassificationTypeNames.Keyword)),

					// Synthetic
					EndOfUsing = new TokenType(103, registry.GetClassificationType(PredefinedClassificationTypeNames.Operator)),
					Type = new TokenType(104, registry.GetClassificationType(PredefinedClassificationTypeNames.SymbolReference)),
					Parameter = new TokenType(105, registry.GetClassificationType(PredefinedClassificationTypeNames.SymbolDefinition)),
					SyntaxName = new TokenType(106, registry.GetClassificationType(PredefinedClassificationTypeNames.String)),
					Namespace = new TokenType(107, registry.GetClassificationType(PredefinedClassificationTypeNames.String)),
				}.ToDictionary(t => t.Id);
			}

			// Defined in syntax
			public readonly TokenType Dot;
			public readonly TokenType Identifier;
			public readonly TokenType Number;
			public readonly TokenType OpenParen;
			public readonly TokenType CloseParen;
			public readonly TokenType ZeroOrMany;
			public readonly TokenType ZeroOrOne;
			public readonly TokenType OneOrMany;
			public readonly TokenType ParameterSeparator;
			public readonly TokenType Syntax;
			public readonly TokenType String;
			public readonly TokenType Using;
			public readonly TokenType EndOfUsing;
			public readonly TokenType GenericArgumentListStart;
			public readonly TokenType GenericArgumentListEnd;

			// Synthetic
			public readonly TokenType Type;
			public readonly TokenType Parameter;
			public readonly TokenType SyntaxName;
			public readonly TokenType Namespace;

			public TokenType this[int id]
			{
				get
				{
					TokenType type;
					return _tokenTypesById.TryGetValue(id, out type) ? type : new TokenType(id, _defaultClassificationType);
				}
			}
		}
	}
}