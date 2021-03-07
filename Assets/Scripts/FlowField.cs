using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowField
{
    private Builder _builder;
    private int _width;
    private int _height;
    private GameObject arrowHolder;
    public bool showDirection = true;

    public FlowField(Builder builder)
    {
        arrowHolder = new GameObject();

        _builder = builder;
        _width = GridController.Width;
        _height = GridController.Height;
    }

    public void Initialise(List<Sector> sectors)
    {
        foreach (Sector sector in sectors)
        {
            sector.ResetSectorFieldValues(_builder.GridController);
        }
    }

    public void CalculateIntegrationField(Vector2Int index)
    {
        Queue<Cell> queue = new Queue<Cell>();
        int value;
        // set currentCell according to the desired destination
        //Cell currentCell = _gridMatrix[index.y, index.x];
        Cell currentCell = GridController.Cell(index);
        currentCell.Value = 0;
        //currentCell.BestValue = 0;

        queue.Enqueue(currentCell);

        while (queue.Count != 0)
        {
            currentCell = queue.Dequeue();

            foreach (Cell neighbourCell in FindNeighbourCells(currentCell))
            {
                value = currentCell.Value + 1;

                if (value < neighbourCell.Value)
                {
                    neighbourCell.Value = value;
                    queue.Enqueue(neighbourCell);
                }
            }
        }
    }

    private IEnumerable<Cell> FindNeighbourCells(Cell currentCell)
    {
        List<Cell> neighbours = new List<Cell>();
        Cell neighbourCell;

        if (currentCell.Index.x > 0)
        {
            neighbourCell = GridController.Cell(currentCell.Index.x - 1, currentCell.Index.y);
            if (neighbourCell != null && neighbourCell.IsActive && neighbourCell.IsWalkable)
            {
                neighbours.Add(neighbourCell);
            }
        }

        if (currentCell.Index.x < (_width - 1))
        {
            neighbourCell = GridController.Cell(currentCell.Index.x + 1, currentCell.Index.y);
            if (neighbourCell != null && neighbourCell.IsActive && neighbourCell.IsWalkable)
            {
                neighbours.Add(neighbourCell);
            }
        }

        if (currentCell.Index.y > 0)
        {
            neighbourCell = GridController.Cell(currentCell.Index.x, currentCell.Index.y - 1);
            if (neighbourCell != null && neighbourCell.IsActive && neighbourCell.IsWalkable)
            {
                neighbours.Add(neighbourCell);
            }
        }

        if (currentCell.Index.y < (_height - 1))
        {
            neighbourCell = GridController.Cell(currentCell.Index.x, currentCell.Index.y + 1);
            if (neighbourCell != null && neighbourCell.IsActive && neighbourCell.IsWalkable)
            {
                neighbours.Add(neighbourCell);
            }
        }

        return neighbours;
    }

    public void CalculateFlowField(GameObject arrow)
    {
        GameObject.Destroy(arrowHolder);
        arrowHolder = new GameObject();

        foreach (Cell cell in GridController.Grid)
        {
            if (cell.IsActive && cell.IsWalkable)
            {
                int value = cell.Value;

                foreach (Cell neighbourCell in FindNeighbourCells(cell))
                {
                    if (neighbourCell.Value < value)
                    {
                        value = neighbourCell.Value;

                        Vector3 direktion = cell.Position - neighbourCell.Position;

                        cell.Direction = direktion;

                        if (showDirection)
                        {
                            GameObject arrowObj = GameObject.Instantiate(arrow, cell.Position, Quaternion.LookRotation(direktion));
                            arrowObj.transform.SetParent(arrowHolder.transform);
                        }
                    }
                }
            }
        }
    }
}