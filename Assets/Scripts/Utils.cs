using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    private static int num;

    public static void DrawCube(int X, int Y, float height = 0.2f, int colorID = 0)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(X, height, Y);

        cube.name = num + "";
        num++;

        Color color = Color.gray;
        switch (colorID)
        {
            case 0:
                break;

            case 1:
                color = Color.red;
                break;

            case 2:
                color = Color.green;
                break;

            case 3:
                color = Color.blue;
                break;

            case 4:
                color = Color.cyan;
                break;
        }
        cube.GetComponent<Renderer>().material.color = color;
    }

    public static Sector[,] CreateSectors(int sectorSize)
    {
        if (CheckSectorSize(sectorSize))
        {
            int width = GridController.Width / sectorSize;
            int height = GridController.Height / sectorSize;

            Sector[,] sectors = new Sector[height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //Debug.Log("Sector " + y + ":" + x);
                    sectors[y, x] = new Sector(new Vector2Int(x, y), sectorSize);
                }
            }
            return sectors;
        }
        Debug.LogError("Sectors could not be created");
        return null;
    }

    private static bool CheckSectorSize(int preferredSectorSize)
    {
        if (GridController.Width % preferredSectorSize == 0 && GridController.Height % preferredSectorSize == 0)
        {
            return true;
        }
        else
        {
            Debug.LogError("Unable to create Sectors - Grid not in power of two");
            return false;
        }
    }

    public static Vector2Int gridPosFromMouse()
    {
        Plane plane = new Plane(Vector3.up, 0);
        Vector2Int gridPos = new Vector2Int();
        float distance;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out distance))
        {
            gridPos.x = Mathf.FloorToInt(ray.GetPoint(distance).x + 0.5f);
            gridPos.y = Mathf.FloorToInt(ray.GetPoint(distance).z + 0.5f);

            //Debug.Log("gridPos: " + gridPos + "# " + ray.GetPoint(distance));

            return gridPos;
        }
        return gridPos;
    }
}