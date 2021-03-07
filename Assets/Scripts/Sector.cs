using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sector
{
    public Vector2Int Index { get; set; }
    public List<Node> Nodes { get; private set; }
    public int Size { get; set; }

    public enum PreferredSectorSize
    {
        Two = 2,
        Four = 4,
        Eight = 8,
        Sixteen = 16
    }

    public Sector(Vector2Int index, int size)
    {
        Index = index;
        Size = size;

        Nodes = new List<Node>();
    }

    public void AddNode(Node node)
    {
        Nodes.Add(node);
    }

    public void ExtrudeSector()
    {
        int offsetX = Index.x * Size;
        int offsetY = Index.y * Size;

        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new Vector3(x + offsetX, 0.5f, y + offsetY);
            }
        }
    }

    public void ResetSectorFieldValues(GridController grid)
    {
        int offsetX = Index.x * Size;
        int offsetY = Index.y * Size;

        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                grid.Value((x + offsetX), (y + offsetY), int.MaxValue);
            }
        }
    }

    public void DeactivateSectorField(GridController grid)
    {
        int offsetX = Index.x * Size;
        int offsetY = Index.y * Size;

        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                grid.IsActive((x + offsetX), (y + offsetY), false);
            }
        }
    }

    public void SetSectorCellsActive(GridController grid)
    {
        int offsetX = Index.x * Size;
        int offsetY = Index.y * Size;

        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                grid.IsActive((x + offsetX), (y + offsetY), true);
            }
        }
    }

    public void ShowSectorCellValue(GridController grid, GameObject buttonPre, Canvas canvas)
    {
        int offsetX = Index.x * Size;
        int offsetY = Index.y * Size;

        GameObject button;
        int value;
        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                button = GameObject.Instantiate(buttonPre);
                button.transform.Rotate(90, 0, 0);
                button.transform.position = new Vector3(x + offsetX, .3f, y + offsetY);

                value = grid.Value((x + offsetX), (y + offsetY));
                if (value < int.MaxValue)
                {
                    button.GetComponentInChildren<Text>().text = "" + value;
                }
                else
                {
                    button.GetComponentInChildren<Text>().text = "§";
                }

                button.transform.SetParent(canvas.transform);
            }
        }
    }
}