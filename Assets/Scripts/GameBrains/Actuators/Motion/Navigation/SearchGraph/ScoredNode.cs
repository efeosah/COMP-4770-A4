using System;
using System.Text;
using C5;

namespace GameBrains.Actuators.Motion.Navigation.SearchGraph
{
	// Used by searches such as Dijkstra's and A*
	public class ScoredNode : IEquatable<ScoredNode>, IShowable
	{
	    public readonly Node node;
	    public readonly float f;
	    public readonly float g;
	    public readonly Edge edgeFromParent;
		public readonly ScoredNode parentScoredNode;

	    public ScoredNode(
		    Node node,
		    float f,
		    float g,
		    Edge edgeFromParent,
		    ScoredNode parentScoredNode)
	    {
	        this.node = node;
	        this.f = f;
	        this.g = g;
	        this.edgeFromParent = edgeFromParent;
			this.parentScoredNode = parentScoredNode;
	    }

	    public bool Equals(ScoredNode other)
	    {
	        return other != null &&
	               (!node ? (!other.node) : node.Equals(other.node)) &&
				f.Equals(other.f) &&
			    g.Equals(other.g) &&
			    (!edgeFromParent
				    ? !other.edgeFromParent
				    : edgeFromParent.Equals(other.edgeFromParent)) &&
			    (parentScoredNode == null
				    ? (other.parentScoredNode == null)
				    : parentScoredNode.Equals(other.parentScoredNode));
	    }

	    public override bool Equals(object obj)
	    {
		    return obj is ScoredNode scoredNode && Equals(scoredNode);
	    }

	    public static bool operator ==(ScoredNode record1, ScoredNode record2)
	    {
	        return record1 is { } && record1.Equals(record2);
	    }

	    public static bool operator !=(ScoredNode record1, ScoredNode record2)
	    {
	        return record1 is { } && !record1.Equals(record2);
	    }

	    // TODO: Research good hash codes.
	    public override int GetHashCode()
	    {
		    int[] primes = { 3049, 5039, 883, 9719 };

		    int parentHashCode = (parentScoredNode == null) ? 0 : parentScoredNode.GetHashCode();
		    int edgeFromParentHashCode = edgeFromParent == null ? 0 : edgeFromParent.GetHashCode();

	        int hashCode = node == null ? 0 : node.GetHashCode();
	        hashCode = hashCode * primes[0] + f.GetHashCode();
	        hashCode = hashCode * primes[1] + g.GetHashCode();
	        hashCode = hashCode * primes[2] + parentHashCode;
	        hashCode = hashCode * primes[3] + edgeFromParentHashCode;

			return hashCode;
	    }

	    public bool Show(StringBuilder stringBuilder, ref int rest, IFormatProvider formatProvider)
	    {
	        bool flag = true;
	        stringBuilder.Append("(");
	        rest -= 2;

	        try
	        {
		        flag = !Showing.Show(node, stringBuilder, ref rest, formatProvider);
	            if (flag) return false;

	            stringBuilder.Append(", ");
	            rest -= 2;

	            flag = !Showing.Show(f, stringBuilder, ref rest, formatProvider);
	            if (flag) return false;

	            stringBuilder.Append(", ");
	            rest -= 2;

	            flag = !Showing.Show(g, stringBuilder, ref rest, formatProvider);
	            if (flag) return false;

	            stringBuilder.Append(", ");
	            rest -= 2;

	            flag = !Showing.Show(edgeFromParent, stringBuilder, ref rest, formatProvider);
	            if (flag) return false;

				stringBuilder.Append(", ");
	            rest -= 2;

	            flag = !Showing.Show(parentScoredNode, stringBuilder, ref rest, formatProvider);
	            if (flag) return false;
	        }
	        finally
	        {
	            if (flag)
	            {
	                stringBuilder.Append("...");
	                rest -= 3;
	            }

	            stringBuilder.Append(")");
	        }

	        return true;
	    }

	    public override string ToString()
	    {
		    return $"({node}, {f}, {g}, {edgeFromParent}, {parentScoredNode})";
	    }

	    public string ToString(string format, IFormatProvider formatProvider)
	    {
	        return Showing.ShowString(this, format, formatProvider);
	    }
	}
}