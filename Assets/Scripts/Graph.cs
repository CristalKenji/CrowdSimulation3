using System;
using System.Collections.Generic;
using System.Text;

public class Graph
{
    public List<Node> Nodes { get; private set; }
    public List<List<AdjListNode>> AdjacenyList { get; private set; }

    public Graph(List<Node> nodes)
    {
        Nodes = nodes;

        InitialiseAdjList();
    }

    private void InitialiseAdjList()
    {
        AdjacenyList = new List<List<AdjListNode>>();

        for (int i = 0; i < Nodes.Count; i++)
        {
            AdjacenyList.Add(new List<AdjListNode>());

            AdjacenyList[i].Add(new AdjListNode(Nodes[i], 0));
        }

        for (int i = 0; i < Nodes.Count - 1; i += 2)
        {
            AdjacenyList[i].Add(new AdjListNode(Nodes[i + 1], 1));
            AdjacenyList[i + 1].Add(new AdjListNode(Nodes[i], 1));
        }
    }

    public void InsertDestinationNode(List<Node> nodes)
    {
        Nodes = nodes;
        AdjacenyList.Add(new List<AdjListNode>());
    }

    public void AddAdjListNode(int nodeID, Node neighbourNode, int distance)
    {
        AdjListNode neigbhour = new AdjListNode(neighbourNode, distance);

        if (!AdjacenyList[nodeID].Contains(neigbhour))
        {
            AdjacenyList[nodeID].Add(neigbhour);
        }
    }

    public string PrintAdjList()
    {
        int length = AdjacenyList.Count;
        StringBuilder stringBuilder = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            AdjacenyList[i].ForEach(delegate (AdjListNode node)
                {
                    stringBuilder.Append(node.ToString() + " ");
                });
            stringBuilder.AppendLine();
        }
        return stringBuilder.ToString();
    }
}