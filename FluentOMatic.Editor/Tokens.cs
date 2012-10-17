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

using System.ComponentModel.Composition;
using System.Windows.Media;
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
				registry.GetClassificationType("FluentOMatic.Dot"),
				registry.GetClassificationType("FluentOMatic.Identifier"),
				registry.GetClassificationType("FluentOMatic.Number"),
				registry.GetClassificationType("FluentOMatic.OpenParen"),
				registry.GetClassificationType("FluentOMatic.CloseParen"),
				registry.GetClassificationType("FluentOMatic.ZeroOrMany"),
				registry.GetClassificationType("FluentOMatic.ZeroOrOne"),
				registry.GetClassificationType("FluentOMatic.OneOrMany"),
				registry.GetClassificationType("FluentOMatic.Comma"),
				registry.GetClassificationType("FluentOMatic.Syntax"),
				registry.GetClassificationType("FluentOMatic.Type"),
				registry.GetClassificationType("FluentOMatic.SyntaxName"),
			};
		}
	}

    internal static class EditorClassificationDefinition
    {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("FluentOMatic.Dot")]
        internal static ClassificationTypeDefinition DotEditorType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("FluentOMatic.Identifier")]
        internal static ClassificationTypeDefinition IdentifierEditorType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("FluentOMatic.Number")]
        internal static ClassificationTypeDefinition NumberEditorType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("FluentOMatic.OpenParen")]
        internal static ClassificationTypeDefinition OpenParenEditorType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("FluentOMatic.CloseParen")]
        internal static ClassificationTypeDefinition CloseParenEditorType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("FluentOMatic.ZeroOrMany")]
        internal static ClassificationTypeDefinition ZeroOrManyEditorType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("FluentOMatic.ZeroOrOne")]
        internal static ClassificationTypeDefinition ZeroOrOneEditorType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("FluentOMatic.OneOrMany")]
        internal static ClassificationTypeDefinition OneOrManyEditorType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("FluentOMatic.Comma")]
        internal static ClassificationTypeDefinition CommaEditorType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("FluentOMatic.Syntax")]
        internal static ClassificationTypeDefinition SyntaxEditorType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("FluentOMatic.Type")]
        internal static ClassificationTypeDefinition TypeEditorType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("FluentOMatic.SyntaxName")]
        internal static ClassificationTypeDefinition SyntaxNameEditorType = null;

    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "FluentOMatic.Dot")]
	[Name("FluentOMatic.Dot")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class DotEditorFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "FluentOMatic.Editor" classification type
        /// </summary>
        public DotEditorFormat()
        {
			DisplayName = "FluentOMatic.Dot"; //human readable version of the name
			ForegroundColor = Colors.Black;
            BackgroundColor = Colors.White;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "FluentOMatic.Identifier")]
	[Name("FluentOMatic.Identifier")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class IdentifierEditorFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "FluentOMatic.Editor" classification type
        /// </summary>
        public IdentifierEditorFormat()
        {
			DisplayName = "FluentOMatic.Identifier"; //human readable version of the name
			ForegroundColor = Colors.Green;
            BackgroundColor = Colors.White;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "FluentOMatic.Number")]
	[Name("FluentOMatic.Number")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class NumberEditorFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "FluentOMatic.Editor" classification type
        /// </summary>
        public NumberEditorFormat()
        {
			DisplayName = "FluentOMatic.Number"; //human readable version of the name
			ForegroundColor = Colors.Blue;
            BackgroundColor = Colors.White;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "FluentOMatic.OpenParen")]
	[Name("FluentOMatic.OpenParen")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class OpenParenEditorFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "FluentOMatic.Editor" classification type
        /// </summary>
        public OpenParenEditorFormat()
        {
			DisplayName = "FluentOMatic.OpenParen"; //human readable version of the name
			ForegroundColor = Colors.Red;
            BackgroundColor = Colors.White;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "FluentOMatic.CloseParen")]
	[Name("FluentOMatic.CloseParen")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class CloseParenEditorFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "FluentOMatic.Editor" classification type
        /// </summary>
        public CloseParenEditorFormat()
        {
			DisplayName = "FluentOMatic.CloseParen"; //human readable version of the name
			ForegroundColor = Colors.Red;
            BackgroundColor = Colors.White;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "FluentOMatic.ZeroOrMany")]
	[Name("FluentOMatic.ZeroOrMany")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class ZeroOrManyEditorFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "FluentOMatic.Editor" classification type
        /// </summary>
        public ZeroOrManyEditorFormat()
        {
			DisplayName = "FluentOMatic.ZeroOrMany"; //human readable version of the name
			ForegroundColor = Colors.Yellow;
            BackgroundColor = Colors.White;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "FluentOMatic.ZeroOrOne")]
	[Name("FluentOMatic.ZeroOrOne")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class ZeroOrOneEditorFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "FluentOMatic.Editor" classification type
        /// </summary>
        public ZeroOrOneEditorFormat()
        {
			DisplayName = "FluentOMatic.ZeroOrOne"; //human readable version of the name
			ForegroundColor = Colors.Yellow;
            BackgroundColor = Colors.White;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "FluentOMatic.OneOrMany")]
	[Name("FluentOMatic.OneOrMany")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class OneOrManyEditorFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "FluentOMatic.Editor" classification type
        /// </summary>
        public OneOrManyEditorFormat()
        {
			DisplayName = "FluentOMatic.OneOrMany"; //human readable version of the name
			ForegroundColor = Colors.Yellow;
            BackgroundColor = Colors.White;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "FluentOMatic.Comma")]
	[Name("FluentOMatic.Comma")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class CommaEditorFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "FluentOMatic.Editor" classification type
        /// </summary>
        public CommaEditorFormat()
        {
			DisplayName = "FluentOMatic.Comma"; //human readable version of the name
			ForegroundColor = Colors.Pink;
            BackgroundColor = Colors.White;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "FluentOMatic.Syntax")]
	[Name("FluentOMatic.Syntax")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class SyntaxEditorFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "FluentOMatic.Editor" classification type
        /// </summary>
        public SyntaxEditorFormat()
        {
			DisplayName = "FluentOMatic.Syntax"; //human readable version of the name
			ForegroundColor = Colors.Cyan;
            BackgroundColor = Colors.White;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "FluentOMatic.Type")]
	[Name("FluentOMatic.Type")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class TypeEditorFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "FluentOMatic.Editor" classification type
        /// </summary>
        public TypeEditorFormat()
        {
			DisplayName = "FluentOMatic.Type"; //human readable version of the name
			ForegroundColor = Colors.Gray;
            BackgroundColor = Colors.White;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "FluentOMatic.SyntaxName")]
	[Name("FluentOMatic.SyntaxName")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class SyntaxNameEditorFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "FluentOMatic.Editor" classification type
        /// </summary>
        public SyntaxNameEditorFormat()
        {
			DisplayName = "FluentOMatic.SyntaxName"; //human readable version of the name
			ForegroundColor = Colors.Lime;
            BackgroundColor = Colors.White;
        }
    }

}
