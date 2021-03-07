using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAgent : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector2Int index;

    private void Start()
    {
        //GameObject.Find("Driver").GetComponent<Driver>().m_reportStartingPoints.AddListener(ReportStartPoint);

        //GameObject.Find("Driver").GetComponent<Driver>().ReportStartPoint(index);
        index.x = (int)transform.position.x;
        index.y = (int)transform.position.z;

        Driver.m_reportStartingPoints.AddListener(ReportStartPoint);
    }

    private void ReportStartPoint()
    {
        // Builder.Grid[x,y].startpoint
        //GameObject
        Debug.Log("Agent " + index + " says: Hola");

        //foreach (AdjListNode node in GridController.Cell(index).TransitPoints)
        //{
        //    Debug.Log(node);
        //}

        //Debug.Log("TP " + GridController.CellTransitPoint(index));
        //Debug.Log("Direction " + GridController.CellDirection(index));

        Pathfinder.AddStartPoint(GridController.CellTransitPoint(index));
    }
}