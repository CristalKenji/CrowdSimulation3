using System;

public struct AdjListNode : IEquatable<AdjListNode>, IComparable<AdjListNode>
{
    public AdjListNode(Node node, int distance)
    {
        Node = node;
        Distance = distance;
    }

    public Node Node { get; }
    public int Distance { get; }

    public int CompareTo(AdjListNode other)
    {
        return Node.CompareTo(other.Node);
    }

    public bool Equals(AdjListNode other)
    {
        return Node.Equals(other.Node);
    }

    public override string ToString()
    {
        return Node.Identifier + "," + Distance;
    }
}