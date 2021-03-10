using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GridController
{
    public static int Width { get; set; }
    public static int Height { get; set; }
    public static Cell[,] Grid { get; private set; }

    public static Node CellTransitPoint(Vector2Int index)
    {
        // returns the TransitPoint of the cell or null if the cell does not exist
        return Cell(index) is null ? null : Cell(index).TransitPoint;
    }

    public static Vector3 CellDirection(Vector2Int index)
    {
        return Cell(index)?.Direction ?? Vector3.zero;
    }

    public static Cell Cell(Vector2Int index)
    {
        return Cell(index.x, index.y);
    }

    public static Cell Cell(int x, int y)
    {
        if ((x >= 0 && x < Width) && (y >= 0 && y < Height))
        {
            return Grid[y, x];
        }
        Debug.LogError("Index out of bounds");
        return null;
    }

    public bool IsWalkable(int x, int y)
    {
        if ((x >= 0 && x < Width) && (y >= 0 && y < Height))
        {
            return Grid[y, x].IsWalkable;
        }
        Debug.LogError("Grid IsWalkable - Coordinate out of bounds");
        return false;
    }

    public int Value(Vector2Int index)
    {
        return Value(index.x, index.y);
    }

    public int Value(int x, int y)
    {
        if ((x >= 0 && x < Width) && (y >= 0 && y < Height))
        {
            return Grid[y, x].Value;
        }
        return -1;
    }

    public void Value(Vector2Int index, int value)
    {
        Value(index.x, index.y, value);
    }

    public void Value(int x, int y, int value)
    {
        if ((x >= 0 && x < Width) && (y >= 0 && y < Height))
        {
            Grid[y, x].Value = value;
        }
    }

    public bool IsActive(int x, int y)
    {
        return Grid[y, x].IsActive;
    }

    public void IsActive(int x, int y, bool value)
    {
        Grid[y, x].IsActive = value;
    }

    public void AddTransitPoint(Vector2Int cellIndex, Node transitPoint, int distance)
    {
        Grid[cellIndex.y, cellIndex.x].AddTransitPoint(transitPoint, distance);
    }

    public GridController(Cell[,] grid)
    {
        Grid = grid;
        Height = grid.GetLength(0);
        Width = grid.GetLength(1);
    }

    public static GridController GridFromTexture(Texture2D importFile)
    {
        if (importFile != null)
        {
            int width = importFile.width;
            int height = importFile.height;
            Cell[,] grid = new Cell[height, width];

            var pixels = importFile.GetPixels32();
            int x = 0;
            int y = 0;

            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].Equals(new Color32(0, 0, 0, 255)))
                {
                    grid[y, x] = new Cell(x, y, false);
                }
                else if (pixels[i].Equals(new Color32(255, 255, 255, 255)))
                {
                    grid[y, x] = new Cell(x, y, true);
                }
                else
                {
                    Debug.LogError("ImportFile - Farbe an Stelle: " + x + " " + y + " nicht erkannt");
                }

                if (x == (width - 1))
                {
                    x = 0;
                    y++;
                }
                else
                {
                    x++;
                }
            }
            return new GridController(grid);
        }
        Debug.LogError("Map.cs - importFileMissing - empty grid returned");
        return new GridController(new Cell[0, 0]);
    }

    public void PrintGrid(bool reverse = false)
    {
        StringBuilder stringBuilder = new StringBuilder(Height * Width);

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                stringBuilder.Append(Grid[y, x] + " ");
            }
            stringBuilder.AppendLine();
        }

        if (reverse)
        {
            Debug.Log(string.Join("\r\n", stringBuilder.ToString().Split('\r', '\n').Reverse()));
        }
        else
        {
            Debug.Log(stringBuilder.ToString());
        }
    }

    public void ExtrudeObstacles()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (!Grid[y, x].IsWalkable)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = new Vector3(x, 0.5f, y);
                    cube.layer = LayerMask.NameToLayer("Obstacle");
                }
            }
        }
    }
}