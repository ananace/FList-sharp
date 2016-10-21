using System;

namespace libflist.Message
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class NodeAttribute : Attribute, IEquatable<NodeAttribute>
	{
		public string Name { get; private set; }
		public NodeValidity Valid { get; set; } = NodeValidity.Both;

		public NodeAttribute(string Name)
		{
			this.Name = Name;
		}

		public bool Equals(NodeAttribute other)
		{
			return Name.Equals(other.Name, StringComparison.CurrentCultureIgnoreCase);
		}
	}
}
