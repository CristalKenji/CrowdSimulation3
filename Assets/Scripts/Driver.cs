using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Driver : MonoBehaviour
{
    public Texture2D _mapTexture;

    [SerializeField] private Sector.PreferredSectorSize _preferredSectorSize = Sector.PreferredSectorSize.Sixteen;
    private int _sectorSize;

    private Builder _builder;
    private Pathfinder _pathfinder;
    private FlowField _flowField;

    private GameObject _destinationCube;

    public GameObject _cellLabel;
    public Canvas _canvas;

    private List<Sector> _flowFieldSectors;

    private GameObject cubeParent;

    public GameObject arrowPrefab;

    public static UnityEvent m_reportStartingPoints;

    public bool showDirection;
    public bool showTransitPoints;
    public bool showCellLabel;

    private void Awake()
    {
        if (m_reportStartingPoints == null)
        {
            m_reportStartingPoints = new UnityEvent();
        }

        cubeParent = new GameObject();

        _flowFieldSectors = new List<Sector>();

        _sectorSize = (int)_preferredSectorSize;

        Builder.showTransitPoints = showTransitPoints;

        _builder = new Builder(_mapTexture, _sectorSize);

        _pathfinder = new Pathfinder(_builder);

        _flowField = new FlowField(_builder);
        _builder.GridController.ExtrudeObstacles();


        //_builder.showTransitPoints = showTransitPoints;
        _flowField.showDirection = showDirection;

        Map._mapTexture = _mapTexture;
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (SetDestination())
            {
                if (m_reportStartingPoints == null)
                {
                    return;
                }

                // Clear data from previous run
                if (_flowFieldSectors.Count > 0)
                {
                    foreach (Sector sector in _flowFieldSectors)
                    {
                        sector.DeactivateSectorField(_builder.GridController);
                    }
                    _flowFieldSectors.Clear();
                }

                _pathfinder.Clear();

                //_pathfinder.AddStartPoint(Builder.Nodes[2]);
                m_reportStartingPoints.Invoke();

                _flowFieldSectors = _pathfinder.FindHighLevelPath();

                _flowField.Initialise(_flowFieldSectors);

                // Debug Cells

                //Destroy(cubeParent);
                //cubeParent = new GameObject();

                //foreach (Cell cell in _builder.Grid.GridMatrix)
                //{
                //    if (cell.IsActive && cell.IsWalkable)
                //    {
                //        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //        cube.transform.position = new Vector3(cell.Index.x, 0.3f, cell.Index.y);
                //        cube.transform.SetParent(cubeParent.transform);
                //    }
                //}

                _flowField.CalculateIntegrationField(_builder.DestinationNode.Coordinate);

                if (showCellLabel)
                {
                    ShowCellLabel();
                }

                _flowField.CalculateFlowField(arrowPrefab);
            }
        }
    }

    private void ShowCellLabel()
    {
        // Clear old Cell Labels
        foreach (Transform cellLabelTransform in _canvas.transform)
        {
            //Debug.Log(cellLabelTransform.name);
            GameObject.Destroy(cellLabelTransform.gameObject);
        }
        // Show Cell Labels
        foreach (Sector sector in _flowFieldSectors)
        {
            sector.ShowSectorCellValue(_builder.GridController, _cellLabel, _canvas);
        }
    }

    private bool SetDestination()
    {
        //Debug.Log("Mouse down");
        Vector2Int destination = Utils.gridPosFromMouse();

        if (_builder.SetDestination(destination)) // Set the mouse pos as destination
        {
            // Visualise destination with a cube
            Destroy(_destinationCube);
            _destinationCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _destinationCube.transform.position = new Vector3(destination.x, 0.2f, destination.y);
            _destinationCube.GetComponent<Renderer>().material.color = Color.magenta;
            _destinationCube.gameObject.layer = LayerMask.NameToLayer("Target");

            return true;
        }
        else
        {
            return false;
        }
    }
}