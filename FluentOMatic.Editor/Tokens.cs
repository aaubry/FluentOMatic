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

using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace FluentOMatic.Editor
{
	partial class Editor
	{
		private IClassificationType[] _classificationTypes;

		internal Editor(IClassificationTypeRegistryService registry)
		{
			_classificationTypes = new[]
			{
				registry.GetClassificationType(PredefinedClassificationTypeNames.Operator),
				registry.GetClassificationType(PredefinedClassificationTypeNames.Identifier),
				registry.GetClassificationType(PredefinedClassificationTypeNames.Number),
				registry.GetClassificationType(PredefinedClassificationTypeNames.Operator),
				registry.GetClassificationType(PredefinedClassificationTypeNames.Operator),
				registry.GetClassificationType(PredefinedClassificationTypeNames.Keyword),
				registry.GetClassificationType(PredefinedClassificationTypeNames.Keyword),
				registry.GetClassificationType(PredefinedClassificationTypeNames.Keyword),
				registry.GetClassificationType(PredefinedClassificationTypeNames.Operator),
				registry.GetClassificationType(PredefinedClassificationTypeNames.Keyword),
				registry.GetClassificationType(PredefinedClassificationTypeNames.SymbolReference),
				registry.GetClassificationType(PredefinedClassificationTypeNames.Keyword),
				registry.GetClassificationType(PredefinedClassificationTypeNames.Operator),
				registry.GetClassificationType(PredefinedClassificationTypeNames.SymbolReference),
				registry.GetClassificationType(PredefinedClassificationTypeNames.SymbolDefinition),
				registry.GetClassificationType(PredefinedClassificationTypeNames.String),
				registry.GetClassificationType(PredefinedClassificationTypeNames.String),
			};
		}

		private static class TokenType
		{
			public const int Dot = 1;
			public const int Identifier = 2;
			public const int Number = 3;
			public const int OpenParen = 4;
			public const int CloseParen = 5;
			public const int ZeroOrMany = 6;
			public const int ZeroOrOne = 7;
			public const int OneOrMany = 8;
			public const int Comma = 9;
			public const int Syntax = 10;
			public const int String = 11;
			public const int Using = 12;
			public const int EndOfUsing = 13;
			public const int Type = 14;
			public const int Parameter = 15;
			public const int SyntaxName = 16;
			public const int Namespace = 17;
		}
	}

    internal static class EditorClassificationDefinition
    {
    }

}
