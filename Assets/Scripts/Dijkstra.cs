using System.Collections.Generic;
using UnityEngine;

public static class Dijkstra
{
    public static List<KeyValuePair<int, int>> CalculatePath(Node start, Node destination, List<List<AdjListNode>> adjacencyList)
    {
        List<KeyValuePair<int, int>> path = new List<KeyValuePair<int, int>>();
        List<Node> sortedNodes = new List<Node>();
        Node currentNode;
        bool foundPath = false;

        Builder.ResetNodeAttributes();
        start.Predecessor = null;
        start.Distance = 0;

        sortedNodes.Add(start);

        while (sortedNodes.Count > 0)
        {
            currentNode = sortedNodes[0];
            sortedNodes.Remove(currentNode);

            if (currentNode.Visited) { continue; }

            if (currentNode.Coordinate == destination.Coordinate) // We have found our destination
            {
                //Debug.Log("You have reached your final destination " + currentNode.Identifier + " = " + destination.Identifier);

                if (currentNode.Identifier == destination.Identifier) // The destination itself - the predecessor must be the nodes predecessor otherwise it references itself
                {
                    destination.Distance = currentNode.Distance;
                    destination.Predecessor = currentNode.Predecessor;
                }
                else // The destination is a transit point and can be set as the destination predecessor
                {
                    destination.Distance = currentNode.Distance;
                    destination.Predecessor = currentNode;
                }
                foundPath = true;
                //break;
            }
            currentNode.Visited = true;
            AddNeighbours(currentNode, sortedNodes, adjacencyList);
        }

        if (foundPath)
        {
            FindPath(destination, start, path);
        }
        else
        {
            Debug.LogWarning("No Path found between " + start.Identifier + " and " + destination.Identifier);
        }
        return path;
    }

    private static void FindPath(Node destination, Node start, List<KeyValuePair<int, int>> path)
    {
        Node node = destination;

        for (int i = 0; i < Builder.Nodes.Count; i++)
        {
            if (node.Predecessor != null)
            {
                path.Add(new KeyValuePair<int, int>(node.Identifier, node.Distance));

                if (node != node.Predecessor)
                {
                    //Debug.Log("Pre " + node.Identifier + " vong " + node.Predecessor);
                    node = node.Predecessor;
                }
            }
            else if (node.Coordinate == start.Coordinate)
            {
                path.Add(new KeyValuePair<int, int>(node.Identifier, node.Distance));
                Debug.Log("Ziel gefunden " + (node.Coordinate + " = " + start.Coordinate));
                break;
            }
        }
    }

    public static void AddNeighbours(Node node, List<Node> sortedNodes, List<List<AdjListNode>> adjacencyList)
    {
        int newDistance;

        adjacencyList[node.Identifier].ForEach(delegate (AdjListNode adjNode)
        {
            // wenn nicht er selber ab in die queue
            if (node.Identifier != adjNode.Node.Identifier && !adjNode.Node.Visited) // !visited streichen? cost muss rekalkuliert werden
            {
                newDistance = node.Distance + adjNode.Distance;

                // found a better path
                if (newDistance < adjNode.Node.Distance)
                {
                    //Debug.Log(" " + adjNode.Node.Identifier + ", " + adjNode.Node.Distance + " new " + newDistance);
                    // remove old entry
                    if (sortedNodes.Contains(adjNode.Node))
                    {
                        sortedNodes.Remove(adjNode.Node);
                        //Debug.Log("Removed " + adjNode.Node.Identifier + ", " + adjNode.Node.Distance + " new " + newDistance);
                    }

                    // update distance
                    adjNode.Node.Distance = newDistance;
                    adjNode.Node.Predecessor = node;

                    // enqueue again
                    sortedNodes.Add(adjNode.Node);
                    sortedNodes.Sort();
                }
            }
        });
    }
}