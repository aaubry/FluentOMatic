
namespace FluentOMatic.Syntax
{
	public class Parameter
	{
		public string Type { get; set; }
		public string Name { get; set; }

		public override string ToString()
		{
			return string.Format("{0}:{1}", Name, Type);
		}
	}
}