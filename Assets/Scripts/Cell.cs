using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public Vector2Int Index { get; set; }
    public bool IsWalkable { get; set; }
    public bool IsActive { get; set; }
    public List<AdjListNode> TransitPoints { get; set; }
    public int Value { get; set; }
    public Vector3 Direction { get; set; }

    public int X { get { return Index.x; } }
    public int Y { get { return Index.y; } }

    public Cell(int x, int y, bool isWalkable) : this(new Vector2Int(x, y), isWalkable)
    {
    }

    public Cell(Vector2Int coordinate, bool isWalkable)
    {
        Index = coordinate;
        IsWalkable = isWalkable;
        TransitPoints = new List<AdjListNode>();
    }

    public void AddTransitPoint(Node node, int distance)
    {
        AdjListNode transitPoint = new AdjListNode(node, distance);

        if (!TransitPoints.Contains(transitPoint))
        {
            TransitPoints.Add(transitPoint);
            TransitPoints.Sort((tp1, tp2) => tp1.Distance.CompareTo(tp2.Distance));
        }
    }

    //public void AddTransitPoint(AdjListNode transitpoint)
    //{
    //    if (!TransitPoints.Contains(transitpoint))
    //        TransitPoints.Add(transitpoint);
    //}

    public Vector3 Position
    {
        get { return new Vector3(X, 0, Y); }
    }

    public Node TransitPoint
    {
        // returns the closest TP Point
        get { return TransitPoints[0].Node; }
    }
}