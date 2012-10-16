
namespace FluentOMatic.Syntax
{
	public interface ISyntaxVisitor
	{
		void Visit(FluentSyntax syntax);
		void Visit(Operation operation);
		void Visit(Parameter parameter);
	}
}