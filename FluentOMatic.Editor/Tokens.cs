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
				registry.GetClassificationType(PredefinedClassificationTypeNames.SymbolDefinition),
				registry.GetClassificationType(PredefinedClassificationTypeNames.String),
			};
		}
	}

    internal static class EditorClassificationDefinition
    {
    }

}
