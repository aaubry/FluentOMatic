﻿using Kebas;
using System;

namespace ConsoleApplication1
{
	class Program
	{
		static void Main(string[] args)
		{
			var s = new _OsdBuilderSyntax();

			((OsdBuilderSyntax)s)
				.Named("name")
				.Described("lang", "description");
			
			
			
			//((OsdBuilderSyntax)s)
			//	.OpenSearchProject(i => i
			//		.Named("a")
			//		.Described("en", "aa")
			//		.Described("pt", "bb")
			//		.WithUrl(u => u
			//			.Named("scheme")
			//			.Template("template")
			//			.Discoverable())
			//	);

			//foreach (var project in s.OpenSearchProject)
			//{
			//	//Console.WriteLine(project.inner.Named.name);
			//	//foreach (var description in project.inner.Described)
			//	//{
			//	//	Console.WriteLine("{0} {1}", description.lang, description.description);
			//	//}
			//	//foreach (var url in project.inner.WithUrl)
			//	//{
			//	//	//Console.WriteLine("{0} {1} {2}", url.scheme, url.template, url.Discoverable);
			//	//	//foreach (var description in url.Described)
			//	//	//{
			//	//	//	Console.WriteLine("{0} {1}", description.lang, description.description);
			//	//	//}
			//	//}
			//}




			//Console.WriteLine(s.OpenSearchProject.inner.Named.name);
			//Console.WriteLine(s.OpenSearchProject.inner.Named.Described.lang);
			//Console.WriteLine(s.OpenSearchProject.inner.Named.Described.description);


			//Console.WriteLine(s.OpenSearchProject.inner.Named.name);
			//Console.WriteLine(s.OpenSearchProject.inner.Named.Described.lang);
			//Console.WriteLine(s.OpenSearchProject.inner.Named.Described.description);

			Console.ReadLine();
		}
	}
}
