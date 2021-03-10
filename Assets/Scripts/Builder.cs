using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder
{
    public GridController GridController { get; private set; }
    public Sector[,] Sectors { get; private set; }

    private Graph _graph;
    public List<List<AdjListNode>> AdjacenyList { get { return _graph.AdjacenyList; } }
    public static List<Node> Nodes { get; private set; }
    public Node DestinationNode { get; private set; }

    private int _sectorSize;

    public static bool showTransitPoints;

    public Builder(Texture2D texture, int sectorSize)
    {
        _sectorSize = sectorSize;
        BuildGraph(texture, sectorSize);
    }

    private void BuildGraph(Texture2D texture, int sectorSize)
    {
        GridController = GridController.GridFromTexture(texture);

        Sectors = Utils.CreateSectors(sectorSize);

        Nodes = DiscoverTransitPoints();

        _graph = new Graph(Nodes);

        DiscoverEdges();

        CreateDestinationNode();
    }

    private void CreateDestinationNode()
    {
        DestinationNode = new Node(0, 0);

        Nodes.Add(DestinationNode);
        _graph.InsertDestinationNode(Nodes);
    }

    public bool SetDestination(Vector2Int coordinate)
    {
        if (coordinate.x >= 0 && coordinate.x < GridController.Width && (coordinate.y >= 0 && coordinate.y < GridController.Height))
        {
            //Debug.Log("SetDestination to " + coordinate);
            DestinationNode.Coordinate = coordinate;

            ResetDestinationEdges(DestinationNode);

            // Check if destination is a transitpoint
            CheckDestination(DestinationNode);

            // Find out in which sector the target is located
            int x = coordinate.x / _sectorSize;
            int y = coordinate.y / _sectorSize;

            // Add destination to the sector
            Sectors[y, x].AddNode(DestinationNode);

            // Connect to its Sector TransitPoints
            foreach (Node transitpoint in Sectors[y, x].Nodes)
            {
                Sectors[y, x].ResetSectorFieldValues(GridController);
                FindConnectedNode(x, y, transitpoint);
            }
            return true;
        }
        else
        {
            Debug.LogError("SetDestination coordinate out of bounds");
            return false;
        }
    }

    public void CheckDestination(Node destination)
    {
        Node node;

        for (int i = 0; i < AdjacenyList[0].Count; i++)
        {
            node = AdjacenyList[0][i].Node;
            if (node.Coordinate == destination.Coordinate)
            {
                Debug.Log("Destination is a transitpoint");
                AdjacenyList[node.Identifier].Insert(1, new AdjListNode(destination, 0));
                AdjacenyList[destination.Identifier].Add(new AdjListNode(node, 0));

                //AdjacenyList.Sort();
            }
        }
    }

    public void ResetDestinationEdges(Node destination)
    {
        _graph.AdjacenyList.ForEach(delegate (List<AdjListNode> adjList)
        {
            for (int i = 1; i < adjList.Count; i++)
            {
                if (adjList[i].Node == destination)
                {
                    adjList.RemoveAt(i);
                }
            }
        });

        _graph.AdjacenyList[DestinationNode.Identifier].Clear();
        _graph.AdjacenyList[DestinationNode.Identifier].Add(new AdjListNode(destination, 0));

        foreach (Sector sector in Sectors)
        {
            if (sector.Nodes.Contains(destination))
            {
                sector.Nodes.Remove(destination);
            }
        }
    }

    public static Node Node(Vector2Int coordinate)
    {
        foreach (Node node in Nodes)
        {
            if (node.Coordinate == coordinate)
            {
                return node;
            }
        }
        //Debug.LogWarning("No Node with the coordinate: " + coordinate + " could be found");
        return null;
    }

    private void DiscoverEdges() // Discovers the connections between each Node in a Sector
    {
        int height = Sectors.GetLength(0);
        int width = Sectors.GetLength(1);

        for (int y = 0; y < height; y++) // height
        {
            for (int x = 0; x < width; x++) // width
            {
                foreach (Node transitpoint in Sectors[y, x].Nodes)
                {
                    //Debug.Log("TP: " + transitpoint.Identifier + " : " + transitpoint.Coordinate);
                    Sectors[y, x].ResetSectorFieldValues(GridController);
                    FindConnectedNode(x, y, transitpoint);
                }
            }
        }
    }

    private void FindConnectedNode(int x, int y, Node startTransitPoint)
    {
        Vector2Int cellIndex;
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        GridController.Value(startTransitPoint.Coordinate.x, startTransitPoint.Coordinate.y, 0);

        //Debug.Log("StartValue " + startTransitPoint.Coordinate + " : " + _grid.Value(startTransitPoint.Coordinate));

        queue.Enqueue(startTransitPoint.Coordinate);

        while (queue.Count > 0)
        {
            cellIndex = queue.Dequeue();

            AddNeighbours(x, y, cellIndex, queue, startTransitPoint);
        }
    }

    private void AddNeighbours(int x, int y, Vector2Int cellIndex, Queue<Vector2Int> queue, Node startTransitPoint)
    {
        int offsetX = x * _sectorSize;
        int offsetY = y * _sectorSize;

        //Debug.Log("PreValue " + cellIndex + _grid.Value(cellIndex));

        if (cellIndex.x - 1 >= offsetX)
        {
            EnqueueCell(new Vector2Int(cellIndex.x - 1, cellIndex.y), GridController.Value(cellIndex), queue, startTransitPoint);
        }
        if (cellIndex.x + 1 < offsetX + _sectorSize)
        {
            EnqueueCell(new Vector2Int(cellIndex.x + 1, cellIndex.y), GridController.Value(cellIndex), queue, startTransitPoint);
        }
        if (cellIndex.y - 1 >= offsetY)
        {
            EnqueueCell(new Vector2Int(cellIndex.x, cellIndex.y - 1), GridController.Value(cellIndex), queue, startTransitPoint);
        }
        if (cellIndex.y + 1 < offsetY + _sectorSize)
        {
            EnqueueCell(new Vector2Int(cellIndex.x, cellIndex.y + 1), GridController.Value(cellIndex), queue, startTransitPoint);
        }
    }

    private void EnqueueCell(Vector2Int cellIndex, int predecessorValue, Queue<Vector2Int> queue, Node startTransitPoint)
    {
        if (GridController.IsWalkable(cellIndex.x, cellIndex.y) && GridController.Value(cellIndex) == int.MaxValue)
        {
            int distance = predecessorValue + 1;

            GridController.Value(cellIndex, distance);

            //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //cube.transform.position = new Vector3(cellIndex.x, 0.3f, cellIndex.y);
            //cube.GetComponent<Renderer>().material.color = Color.red;
            //Destroy(cube, 20);

            //Add TP Point to Cell
            if (startTransitPoint != DestinationNode)
                GridController.AddTransitPoint(cellIndex, startTransitPoint, distance);

            //if cell is also a tp -> Update Adjazenzymatrix
            Node transitpoint = Node(cellIndex);
            if (transitpoint != null)
            {
                //Debug.Log("Connect " + startTransitPoint.Identifier + " to " + tp.Identifier + " dist: " + (predecessorValue + 1));
                if (transitpoint != DestinationNode)
                    GridController.AddTransitPoint(cellIndex, transitpoint, 0);
                // AdjList
                _graph.AddAdjListNode(startTransitPoint.Identifier, transitpoint, distance);
            }
            queue.Enqueue(cellIndex);
        }
    }

    private List<Node> DiscoverTransitPoints() // Determents which Cells are possible Transitpoints and cretes Nodes for the Graph accordingly
    {
        int height = Sectors.GetLength(0);
        int width = Sectors.GetLength(1);

        List<Node> nodes = new List<Node>();

        for (int y = 0; y < height; y++) //height
        {
            for (int x = 0; x < width; x++) //width
            {
                // actual findTPFunction
                if (x + 1 < width)
                {
                    checkBorder(x, y, true, nodes);
                }

                if (y + 1 < height)
                {
                    checkBorder(x, y, false, nodes);
                }
            }
        }
        return nodes;
    }

    private void checkBorder(int x, int y, bool checkHorizontal, List<Node> nodes)
    {
        int transitionSpace = 0;
        int indexX, indexY;
        int sectorX, sectorY;

        List<Node> transitPointPair;

        if (checkHorizontal)
        {
            sectorX = _sectorSize + ((x * _sectorSize) - 1);
            sectorY = y + (y * (_sectorSize - 1));

            for (int i = 0; i < _sectorSize; i++)
            {
                indexY = sectorY + i;

                if (GridController.IsWalkable(sectorX, indexY) && GridController.IsWalkable(sectorX + 1, indexY))
                {
                    transitionSpace++;

                    if (i == (_sectorSize - 1))
                    {
                        transitPointPair = CreateTransitPointPair(sectorX, indexY, transitionSpace, true, x, y);
                        JoinNodeList(nodes, transitPointPair);
                        transitionSpace = 0;
                    }
                }
                else
                {
                    transitPointPair = CreateTransitPointPair(sectorX, indexY - 1, transitionSpace, true, x, y);
                    JoinNodeList(nodes, transitPointPair);
                    transitionSpace = 0;
                }
            }
        }
        else
        {
            sectorX = x + (x * (_sectorSize - 1));
            sectorY = _sectorSize + ((y * _sectorSize) - 1);

            for (int i = 0; i < _sectorSize; i++)
            {
                indexX = sectorX + i;

                if (GridController.IsWalkable(indexX, sectorY) && GridController.IsWalkable(indexX, sectorY + 1))
                {
                    transitionSpace++;

                    if (i == (_sectorSize - 1))
                    {
                        transitPointPair = CreateTransitPointPair(indexX, sectorY, transitionSpace, false, x, y);
                        JoinNodeList(nodes, transitPointPair);
                        transitionSpace = 0;
                    }
                }
                else
                {
                    transitPointPair = CreateTransitPointPair(indexX - 1, sectorY, transitionSpace, false, x, y);
                    JoinNodeList(nodes, transitPointPair);
                    transitionSpace = 0;
                }
            }
        }
    }

    private void JoinNodeList(List<Node> nodes, List<Node> nodePairs)
    {
        if (nodePairs != null && nodePairs.Count > 0)
        {
            nodes.AddRange(nodePairs);
        }
    }

    private List<Node> CreateTransitPointPair(int x, int y, int length, bool leftRight, int sectorIndexX, int sectorIndexY)
    {
        int indexX, indexY;
        Node transitPointLeft, transitPointRight;
        List<Node> nodePair = new List<Node>();

        if (length < 6 && length > 0)
        {
            if (leftRight)
            {
                indexY = y - Mathf.CeilToInt(length / 2);

                transitPointLeft = new Node(x, indexY);
                transitPointRight = new Node(x + 1, indexY);

                Sectors[sectorIndexY, sectorIndexX].AddNode(transitPointLeft);
                Sectors[sectorIndexY, sectorIndexX + 1].AddNode(transitPointRight);

                if (showTransitPoints)
                {
                    Utils.DrawCube(x, indexY, 1, 2);
                    Utils.DrawCube(x + 1, indexY, 1, 2);
                }

                nodePair.Add(transitPointLeft);
                nodePair.Add(transitPointRight);

                return nodePair;
            }
            else
            {
                indexX = x - Mathf.CeilToInt(length / 2);

                transitPointLeft = new Node(indexX, y);
                transitPointRight = new Node(indexX, y + 1);

                Sectors[sectorIndexY, sectorIndexX].AddNode(transitPointLeft);
                Sectors[sectorIndexY + 1, sectorIndexX].AddNode(transitPointRight);

                if (showTransitPoints)
                {
                    Utils.DrawCube(indexX, y, 1, 2);
                    Utils.DrawCube(indexX, y + 1, 1, 2);
                }

                nodePair.Add(transitPointLeft);
                nodePair.Add(transitPointRight);

                return nodePair;
            }
        }
        else if (length >= 6)
        {
            if (leftRight)
            {
                transitPointLeft = new Node(x, y);
                transitPointRight = new Node(x + 1, y);

                Sectors[sectorIndexY, sectorIndexX].AddNode(transitPointLeft);
                Sectors[sectorIndexY, sectorIndexX + 1].AddNode(transitPointRight);

                nodePair.Add(transitPointLeft);
                nodePair.Add(transitPointRight);

                if (showTransitPoints)
                {
                    Utils.DrawCube(x, y, 1, 2);
                    Utils.DrawCube(x + 1, y, 1, 2);
                }

                indexY = y - (length - 1);

                transitPointLeft = new Node(x, indexY);
                transitPointRight = new Node(x + 1, indexY);

                Sectors[sectorIndexY, sectorIndexX].AddNode(transitPointLeft);
                Sectors[sectorIndexY, sectorIndexX + 1].AddNode(transitPointRight);

                if (showTransitPoints)
                {
                    Utils.DrawCube(x, indexY, 1, 2);
                    Utils.DrawCube(x + 1, indexY, 1, 2);
                }

                nodePair.Add(transitPointLeft);
                nodePair.Add(transitPointRight);

                return nodePair;
            }
            else
            {
                transitPointLeft = new Node(x, y);
                transitPointRight = new Node(x, y + 1);

                Sectors[sectorIndexY, sectorIndexX].AddNode(transitPointLeft);
                Sectors[sectorIndexY + 1, sectorIndexX].AddNode(transitPointRight);

                if (showTransitPoints)
                {
                    Utils.DrawCube(x, y, 1, 2);
                    Utils.DrawCube(x, y + 1, 1, 2);
                }

                nodePair.Add(transitPointLeft);
                nodePair.Add(transitPointRight);

                indexX = x - (length - 1);

                transitPointLeft = new Node(indexX, y);
                transitPointRight = new Node(indexX, y + 1);

                Sectors[sectorIndexY, sectorIndexX].AddNode(transitPointLeft);
                Sectors[sectorIndexY + 1, sectorIndexX].AddNode(transitPointRight);

                if (showTransitPoints)
                {
                    Utils.DrawCube(indexX, y, 1, 2);
                    Utils.DrawCube(indexX, y + 1, 1, 2);
                }

                nodePair.Add(transitPointLeft);
                nodePair.Add(transitPointRight);

                return nodePair;
            }
        }
        return null;
    }

    public string AdjListToString()
    {
        return _graph.PrintAdjList();
    }

    public static void ResetNodeAttributes()
    {
        for (int i = 0; i < Nodes.Count; i++)
        {
            Nodes[i].Distance = int.MaxValue;
            Nodes[i].Visited = false;
            Nodes[i].Predecessor = null;
        }
    }
}