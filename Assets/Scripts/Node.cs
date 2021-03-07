using System;
using UnityEngine;

public class Node : IEquatable<Node>, IComparable<Node>
{
    private static int numberOfNodes;
    public int Identifier { get; private set; }
    public Vector2Int Coordinate { get; set; }
    public int X { get => Coordinate.x; }
    public int Y { get => Coordinate.y; }
    public int Distance { get; set; }
    public bool Visited { get; set; }
    public Node Predecessor { get; set; }

    public Node(int x, int y) : this(new Vector2Int(x, y))
    {
    }

    public Node(Vector2Int coordinate)
    {
        Identifier = numberOfNodes++;
        Coordinate = coordinate;
        Distance = int.MaxValue;
        Visited = false;
        Predecessor = null;
    }

    public int CompareTo(Node other)
    {
        if (Distance == other.Distance)
        {
            return 0;
        }
        else if (Distance > other.Distance)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

    public override string ToString()
    {
        return "ID: " + Identifier + " Dist: " + Distance;
    }

    public bool Equals(Node other)
    {
        if (other == null) return false;

        if (Identifier == other.Identifier)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override int GetHashCode()
    {
        return Identifier;
    }
}