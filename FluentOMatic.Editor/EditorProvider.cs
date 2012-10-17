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

using EnvDTE;
using FluentOMatic.Emission;
using FluentOMatic.States;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;

namespace FluentOMatic.Editor
{
	/// <summary>
	/// This class causes a classifier to be added to the set of classifiers. Since 
	/// the content type is set to "text", this classifier applies to all text files
	/// </summary>
	[Export(typeof(IClassifierProvider))]
	[ContentType("fluent")]
	internal class EditorProvider : IClassifierProvider
	{
		/// <summary>
		/// Import the classification registry to be used for getting a reference
		/// to the custom classification type later.
		/// </summary>
		[Import]
		internal IClassificationTypeRegistryService ClassificationRegistry = null; // Set via MEF

		private readonly DocumentEvents _documentEvents;

		[ImportingConstructor]
		public EditorProvider(SVsServiceProvider serviceProvider)
		{
			var dte = (_DTE)serviceProvider.GetService(typeof(_DTE));
			_documentEvents = dte.Events.DocumentEvents;
			_documentEvents.DocumentSaved += OnDocumentSaved;
		}

		public void OnDocumentSaved(Document document)
		{
			if (document.FullName.EndsWith(FileAndContentTypeDefinitions.FileExtension, StringComparison.OrdinalIgnoreCase))
			{
				var generatedFileName = document.FullName + ".cs";

				var parser = new Parser(new Scanner(document.FullName));
				parser.errors.errorStream = new StringWriter();
				parser.Parse();

				if (parser.errors.count != 0)
				{
					File.WriteAllText(generatedFileName, parser.errors.errorStream.ToString());
				}
				else
				{
					var graphBuilder = new StateGraphBuilder();
					var states = graphBuilder.BuildGraph(parser.Syntax);

					using (var output = File.CreateText(generatedFileName))
					{
						var generator = new CodeGenerator();
						generator.GenerateCode(states.First(), output, parser.Syntax.Name);
					}
				}

				var fileAlreadyAdded = false;
				var projectItems = document.ProjectItem.ProjectItems;
				for (int i = 0; i < projectItems.Count; i++)
				{
					var item = (ProjectItem)projectItems.Item(i + 1);
					if (item.Document.FullName.Equals(generatedFileName, StringComparison.OrdinalIgnoreCase))
					{
						fileAlreadyAdded = true;
						break;
					}
				}
				if (!fileAlreadyAdded)
				{
					projectItems.AddFromFile(generatedFileName);
				}
			}
		}

		public IClassifier GetClassifier(ITextBuffer buffer)
		{
			return buffer.Properties.GetOrCreateSingletonProperty<Editor>(delegate { return new Editor(ClassificationRegistry); });
		}
	}
}