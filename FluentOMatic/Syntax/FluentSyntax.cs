
namespace FluentOMatic.Syntax
{
	public class FluentSyntax
	{
		public string Name { get; set; }
		public OperationList Operations { get; private set; }

		public FluentSyntax()
		{
			Operations = new OperationList();
		}
	}
}
