
namespace FluentOMatic.Syntax
{
	public abstract class SyntaxElement
	{
		public abstract void Accept(ISyntaxVisitor visitor);
	}
}
