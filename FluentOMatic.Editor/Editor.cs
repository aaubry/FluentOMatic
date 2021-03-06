﻿//    Copyright 2012 Antoine Aubry
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

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FluentOMatic.Editor
{
	/// <summary>
	/// Classifier that classifies all text as an instance of the OrinaryClassifierType
	/// </summary>
	internal partial class Editor : IClassifier
	{
		/// <summary>
		/// This method scans the given SnapshotSpan for potential matches for this classification.
		/// In this instance, it classifies everything and returns each span as a new ClassificationSpan.
		/// </summary>
		/// <param name="trackingSpan">The span currently being classified</param>
		/// <returns>A list of ClassificationSpans that represent spans identified to be of this classification</returns>
		public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
		{
			var scanner = new Scanner(new MemoryStream(Encoding.UTF8.GetBytes(span.GetText())));

			var classifications = new List<ClassificationSpan>();
			Token token;
			bool isNamespace = false;
			int previousTokenKind = 0;
			while ((token = scanner.Scan()).kind != 0)
			{
				var kind = token.kind;
				if(kind == _tokenTypes.Identifier.Id)
				{
					if (scanner.Peek().kind == _tokenTypes.Identifier.Id)
					{
						kind = _tokenTypes.Type.Id;
					}
					else if (previousTokenKind == _tokenTypes.Type.Id)
					{
						kind = _tokenTypes.Parameter.Id;
					}
					else if (previousTokenKind == _tokenTypes.Syntax.Id)
					{
						kind = _tokenTypes.SyntaxName.Id;
					}
					else if (previousTokenKind == _tokenTypes.Using.Id || isNamespace)
					{
						kind = _tokenTypes.Namespace.Id;
						isNamespace = true;
					}
				}

				classifications.Add(
					new ClassificationSpan(
						new SnapshotSpan(
							span.Snapshot,
							new Span(span.Start + token.charPos, token.val.Length)
						),
						_tokenTypes[kind].ClassificationType
					)
				);

				previousTokenKind = kind;
			}

			return classifications;
		}

#pragma warning disable 67
		// This event gets raised if a non-text change would affect the classification in some way,
		// for example typing /* would cause the classification to change in C# without directly
		// affecting the span.
		public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
#pragma warning restore 67
	}
}