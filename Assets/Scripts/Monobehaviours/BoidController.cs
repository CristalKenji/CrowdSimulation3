using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Profiling;


public class BoidController : MonoBehaviour
{
    #region Movement Variables
    internal HashSet<Transform> boidsToConsider = new HashSet<Transform>();
    public Vector3 direction, cohesion, alignment, separation, obstacleEvasion, target;
    internal Vector3 cohesionSteeringDelta, alignmentSteeringDelta, separationSteeringDelta, obstacleEvasionSteeringDelta, targetSteeringDelta;
    internal Vector3 currentVelocity, combinedVelocityDelta;
    [SerializeField]
    private int boidsConsidered = 0;

    public Driver driver;
    #endregion

    #region Collision Helpers
    internal bool headingForCollision, collided;
    public int collisions = 0;
    internal static Vector3[] fibonacciSphere = new Vector3[0];
    internal Vector3[] FibonacciSphere
    {
        get
        {
            if (fibonacciSphere.Length != settings.ObstacleRays)
            {
                return fibonacciSphere = FibonacciSpiralHelper.GenerateUnitSphere(settings.ObstacleRays);
            }
            return fibonacciSphere;
        }
    }
    #endregion


    public BoidSettings settings;

    private MeshRenderer meshRenderer;

    void Start()
    {
        currentVelocity = transform.forward * (settings.MinVelocity + settings.MaxVelocity) / 2;
        meshRenderer = GetComponent<MeshRenderer>();
        //boidsCached = BoidSpawner.Instance.boids;
        //Driver.m_reportStartingPoints.AddListener(pleaseSubscribe);
    }

    void pleaseSubscribe()
    {
    }

    Collider[] boidColliderOverlap = new Collider[32];
    int boidColliderCount = 0;
    void Update()
    {
        boidsToConsider.Clear();
        //maybe do this based on distance alone so no physics are involved?
        boidColliderCount = Physics.OverlapSphereNonAlloc(transform.position, settings.AwarenessRadius, boidColliderOverlap, settings.BoidMask);
        for (int i = 0; i < boidColliderCount; i++)
        {
            boidsToConsider.Add(boidColliderOverlap[i].transform);
        }

        // for (int i = 0; i < boidsCached.Length; i++)
        // {
        //     if ((boidsCached[i].transform.position - transform.position).sqrMagnitude < settings.AwarenessRadius * settings.AwarenessRadius)
        //         boidsToConsider.Add(boidsCached[i].gameObject);
        // }

        boidsToConsider.Remove(transform);
        boidsConsidered = boidsToConsider.Count;

        headingForCollision = CheckForCollision();

        combinedVelocityDelta = AggregateForces();

        currentVelocity += combinedVelocityDelta * Time.deltaTime;
        direction = currentVelocity.normalized;
        float speed = currentVelocity.magnitude;
        //Vector3 clamp can only clamp max 
        speed = Mathf.Clamp(speed, settings.MinVelocity, settings.MaxVelocity);
        currentVelocity = direction * speed;
        transform.position += currentVelocity * Time.deltaTime;
        transform.forward = direction;
    }

    BoidController[] boidsCached;


    private void OnCollisionEnter(Collision other)
    {

        if (((1 << other.gameObject.layer) & settings.ObstacleMask) != 0)
        {
            collided = true;
            collisions++;
            outOfBounds();

        }
        if (((1 << other.gameObject.layer) & settings.TargetMask) != 0)
        {
            outOfBounds();
        }
    }

    /// Sum up all forces from rules with their respective weight
    private Vector3 AggregateForces()
    {

        if (!settings.UseOptimizedRules)
        {
            cohesion = Cohesion();
            alignment = AlignmentDirection();
            separation = Separation();
        }
        else
        {
            OptimizedRules(out alignment, out cohesion, out separation);
        }
        obstacleEvasion = headingForCollision ? ObstacleAvoidance() : Vector3.zero;

        target = GridController.Cell(new Vector2Int(Mathf.FloorToInt(ownPos.x + 0.5f), Mathf.FloorToInt(ownPos.z + 0.5f)))?.Direction ?? outOfBounds();

        if (target == Vector3.zero)
        {
            Pathfinder.AddStartPoint(GridController.CellTransitPoint(new Vector2Int(Mathf.FloorToInt(ownPos.x + 0.5f), Mathf.FloorToInt(ownPos.z + 0.5f))));

            driver?.RefreshPaths();
        }

        cohesionSteeringDelta = CalculateSteeringDelta(cohesion, currentVelocity, settings.MaxSteerForce) * settings.CohesionWeight;
        alignmentSteeringDelta = CalculateSteeringDelta(alignment, currentVelocity, settings.MaxSteerForce) * settings.AlignmentWeight;
        separationSteeringDelta = CalculateSteeringDelta(separation, currentVelocity, settings.MaxSteerForce) * settings.SeparationWeight;
        obstacleEvasionSteeringDelta = headingForCollision ? CalculateSteeringDelta(obstacleEvasion, currentVelocity, settings.MaxSteerForce) * settings.ObstacleAvoidanceWeight : Vector3.zero;
        targetSteeringDelta = CalculateSteeringDelta(-target, currentVelocity, settings.MaxSteerForce) * settings.TargetWeight;


        Vector3 summedSteeringDeltas = cohesionSteeringDelta + alignmentSteeringDelta + separationSteeringDelta + obstacleEvasionSteeringDelta + targetSteeringDelta;
        return summedSteeringDeltas;
    }

    private Vector3 outOfBounds()
    {
        BoidSpawner.QueueBoid(gameObject);
        return Vector3.zero;
    }

    // Convert a force into a steering delta
    private Vector3 CalculateSteeringDelta(Vector3 desiredVelocity, Vector3 currentVelocity, float maxSteeringDelta)
    {
        // Calculate Delta between desired and current velocity
        return Vector3.ClampMagnitude((desiredVelocity.normalized * settings.MaxVelocity) - currentVelocity, maxSteeringDelta);
    }

    #region Movement Rules

    /// Returns average direction of the surrounding boids
    private Vector3 AlignmentDirection()
    {
        if (boidsToConsider.Count == 0)
            return transform.forward;

        Vector3 localAlignment = Vector3.zero;

        foreach (Transform neighbour in boidsToConsider)
        {
            localAlignment += neighbour.forward;
        }
        return localAlignment.normalized;
    }

    /// Returns vector to the average position of surrounding boids
    private Vector3 Cohesion()
    {
        Vector3 localCohesion = Vector3.zero;

        // Early out
        if (boidsToConsider.Count == 0)
            return localCohesion;

        foreach (Transform neighbour in boidsToConsider)
        {
            localCohesion += neighbour.position;
        }
        localCohesion = localCohesion / boidsToConsider.Count - transform.position;
        return localCohesion;
    }

    /// Returns inversely proportional vector away from surrounding boids
    private Vector3 Separation()
    {
        Vector3 localSeparation = Vector3.zero;
        if (boidsToConsider.Count == 0)
            return localSeparation;
        foreach (Transform neighbour in boidsToConsider)
        {
            Vector3 neighbourToThis = transform.position - neighbour.position;

            //Inversely Proportional force, closer boids cause higher force
            localSeparation += neighbourToThis.normalized - (neighbourToThis / settings.AwarenessRadius);
        }
        localSeparation /= boidsToConsider.Count;

        return localSeparation;
    }

    Vector3 neighbourPos, neighbourDir, ownPos;

    private void OptimizedRules(out Vector3 alignment, out Vector3 cohesion, out Vector3 separation)
    {

        alignment = transform.forward;
        cohesion = Vector3.zero;
        separation = Vector3.zero;
        ownPos = transform.position;

        if (boidsToConsider.Count == 0)
        {
            return;
        }

        Vector3 neighbourToThis = Vector3.zero;
        foreach (Transform neighbour in boidsToConsider)
        {

            neighbourPos = neighbour.position;
            neighbourDir = neighbour.forward;

            if (settings.StripYAxis)
            {
                alignment.y = 0;
                neighbourPos.y = 0;
                neighbourDir.y = 0;
                neighbourDir = neighbourDir.normalized;
                ownPos.y = 0;
            }

            alignment += neighbourDir;
            cohesion += neighbourPos;
            neighbourToThis = ownPos - neighbourPos;
            separation += neighbourToThis.normalized - (neighbourToThis / settings.AwarenessRadius);
        }

        alignment = alignment.normalized;
        cohesion = cohesion / boidsToConsider.Count - ownPos;
        separation /= boidsToConsider.Count;
    }

    Vector3 modifiedDir;
    private Vector3 ObstacleAvoidance()
    {
        foreach (Vector3 dir in FibonacciSphere)
        {
            modifiedDir = dir;
            if (settings.StripYAxis)
            {
                modifiedDir.y = 0;
                modifiedDir = modifiedDir.normalized;
            }
            if (Physics.SphereCastNonAlloc(transform.position, settings.ObstacleDetectionSphereRadius, transform.TransformDirection(modifiedDir), hits, settings.ObstacleDetectionRange, settings.ObstacleMask) == 0)
                return transform.TransformDirection(modifiedDir);
        }
        return transform.forward;
    }

    #endregion

    RaycastHit[] hits = new RaycastHit[1];
    Collider[] cols = new Collider[1];

    private bool CheckForCollision()
    {
        //if heading for collision, return true
        if (0 != Physics.SphereCastNonAlloc(transform.position, settings.ObstacleDetectionSphereRadius, transform.forward, hits, settings.ObstacleDetectionRange, Physics.AllLayers ^ settings.ObstacleMask))
        {
            return true;
        }
        //if not heading for collision but did so last frame, check if a wall is too close
        if (headingForCollision)
        {
            return Physics.OverlapSphereNonAlloc(transform.position, settings.ObstacleDetectionSphereRadius, cols, Physics.AllLayers ^ settings.ObstacleMask) > 0;
        }
        return false;
    }
}


