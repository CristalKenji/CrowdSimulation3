using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Graph;

public class Pathfinder
{
    private List<List<KeyValuePair<int, int>>> _paths;

    public static List<Node> StartingPoints { get; private set; }
    public Node Destination { get; set; }

    private Builder _builder;

    public Pathfinder(Builder builder)
    {
        _builder = builder;
        _paths = new List<List<KeyValuePair<int, int>>>();
        StartingPoints = new List<Node>();
    }

    public List<Sector> FindHighLevelPath()
    {
        //foreach starting point
        foreach (Node start in StartingPoints)
        {
            List<KeyValuePair<int, int>> path = Dijkstra.CalculatePath(start, _builder.DestinationNode, _builder.AdjacenyList);

            if (path.Count > 0)
            {
                _paths.Add(path);
            }
            else
            {
                _builder.ResetDestinationEdges(_builder.DestinationNode);
            }
        }

        //PrintPaths();
        return SectorsFromPaths();
        // Evaluate which sectors need to be traversed
    }

    public List<Sector> SectorsFromPaths()
    {
        List<Sector> sectors = new List<Sector>();

        // Determents which sectors are traversed by the paths
        List<Vector2Int> sectorIndices = new List<Vector2Int>();

        for (int i = 0; i < _paths.Count; i++)
        {
            foreach (KeyValuePair<int, int> nodeDistancePair in _paths[i])
            {
                // Find the sector related to the Node location
                foreach (Sector sector in _builder.Sectors)
                {
                    if (FindNodeInSector(sector, nodeDistancePair, sectorIndices)) break;
                }
            }
        }

        // Marks the necessary cells as active
        foreach (Vector2Int index in sectorIndices)
        {
            //Debug.Log(index);
            //_sectors[index.y, index.x].ExtrudeSector();
            _builder.Sectors[index.y, index.x].SetSectorCellsActive(_builder.GridController);

            sectors.Add(_builder.Sectors[index.y, index.x]);
        }
        return sectors;
    }

    private bool FindNodeInSector(Sector sector, KeyValuePair<int, int> nodeDistancePair, List<Vector2Int> sectorIndices)
    {
        foreach (Node node in sector.Nodes)
        {
            if (node.Identifier == nodeDistancePair.Key)
            {
                // sector allready known? if not add to list
                if (!sectorIndices.Contains(sector.Index))
                {
                    //Debug.Log(node.Identifier + " == " + nodeDistancePair.Key + " in " + sector.Index);
                    sectorIndices.Add(sector.Index);
                }
                return true;
            }
        }
        return false;
    }

    public static void AddStartPoint(Node node)
    {
        if (node != null && !StartingPoints.Contains(node))
        {
            StartingPoints.Add(node);
        }
    }

    private void PrintPaths()
    {
        for (int i = 0; i < _paths.Count; i++)
        {
            Debug.Log("Path " + i + "-----------------");
            for (int j = 0; j < _paths[i].Count; j++)
            {
                Debug.Log(_paths[i][j]);
            }
        }
    }

    public void Clear()
    {
        _paths.Clear();
        StartingPoints.Clear();
    }
}