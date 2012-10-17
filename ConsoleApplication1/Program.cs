using Osd.Syntax;
using System;

namespace ConsoleApplication1
{
	class Program
	{
		static void Main(string[] args)
		{
			var s = new OsdBuilder();

			((OsdSyntax)s)
				.OpenSearchProject("xxx", i => i
					.Named("a")
					.Described("en", "aa")
					.Described("pt", "bb")
					.WithUrl(u => u
						.Named("scheme")
						.Template("template")
						.Discoverable())
				);

			foreach (var project in s.OpenSearchProject)
			{
				Console.WriteLine(project.name);
				Console.WriteLine(project.inner.Named.name);
				foreach (var description in project.inner.Described)
				{
					Console.WriteLine("{0} {1}", description.lang, description.description);
				}
				foreach (var url in project.inner.WithUrl)
				{
					Console.WriteLine("{0} {1} {2}", url.inner.Named.scheme, url.inner.Template.template, url.inner.Discoverable != null);
					foreach (var description in url.inner.Described)
					{
						Console.WriteLine("{0} {1}", description.lang, description.description);
					}
				}
			}
	
			Console.ReadLine();
		}
	}
}
