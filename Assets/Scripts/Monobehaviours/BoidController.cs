using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Profiling;


public class BoidController : MonoBehaviour
{
    #region Movement Variables
    internal HashSet<GameObject> boidsToConsider = new HashSet<GameObject>();
    public Vector3 direction, cohesion, alignment, separation, obstacleEvasion;
    internal Vector3 cohesionSteeringDelta, alignmentSteeringDelta, separationSteeringDelta, obstacleEvasionSteeringDelta;
    internal Vector3 currentVelocity, combinedVelocityDelta;
    [SerializeField]
    private int boidsConsidered = 0;
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
        boidsCached = BoidSpawner.Instance.boids;
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
            boidsToConsider.Add(boidColliderOverlap[i].gameObject);
        }

        // for (int i = 0; i < boidsCached.Length; i++)
        // {
        //     if ((boidsCached[i].transform.position - transform.position).sqrMagnitude < settings.AwarenessRadius * settings.AwarenessRadius)
        //         boidsToConsider.Add(boidsCached[i].gameObject);
        // }

        boidsToConsider.Remove(gameObject);
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
            Debug.Log("Collision");
            meshRenderer.material = settings.boidMaterialEscaped;
            transform.position = Vector3.zero;
            //Debug.Break();
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

        cohesionSteeringDelta = CalculateSteeringDelta(cohesion, currentVelocity, settings.MaxSteerForce) * settings.CohesionWeight;
        alignmentSteeringDelta = CalculateSteeringDelta(alignment, currentVelocity, settings.MaxSteerForce) * settings.AlignmentWeight;
        separationSteeringDelta = CalculateSteeringDelta(separation, currentVelocity, settings.MaxSteerForce) * settings.SeparationWeight;
        obstacleEvasionSteeringDelta = headingForCollision ? CalculateSteeringDelta(obstacleEvasion, currentVelocity, settings.MaxSteerForce) * settings.ObstacleAvoidanceWeight : Vector3.zero;

        Vector3 summedSteeringDeltas = cohesionSteeringDelta + alignmentSteeringDelta + separationSteeringDelta + obstacleEvasionSteeringDelta;

        return summedSteeringDeltas;
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

        foreach (GameObject neighbour in boidsToConsider)
        {
            localAlignment += neighbour.transform.forward;
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

        foreach (GameObject neighbour in boidsToConsider)
        {
            localCohesion += neighbour.transform.position;
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
        foreach (GameObject neighbour in boidsToConsider)
        {
            Vector3 neighbourToThis = transform.position - neighbour.transform.position;

            //Inversely Proportional force, closer boids cause higher force
            localSeparation += neighbourToThis.normalized - (neighbourToThis / settings.AwarenessRadius);
        }
        localSeparation /= boidsToConsider.Count;

        return localSeparation;
    }

    private void OptimizedRules(out Vector3 alignment, out Vector3 cohesion, out Vector3 separation)
    {

        alignment = transform.forward;
        cohesion = Vector3.zero;
        separation = Vector3.zero;

        if (boidsToConsider.Count == 0)
        {
            return;
        }

        Vector3 neighbourToThis = Vector3.zero;
        foreach (GameObject neighbour in boidsToConsider)
        {
            alignment += neighbour.transform.forward;
            cohesion += neighbour.transform.position;
            neighbourToThis = transform.position - neighbour.transform.position;
            separation += neighbourToThis.normalized - (neighbourToThis / settings.AwarenessRadius);
        }

        alignment = alignment.normalized;
        cohesion = cohesion / boidsToConsider.Count - transform.position;
        separation /= boidsToConsider.Count;
    }

    private Vector3 ObstacleAvoidance()
    {
        foreach (Vector3 dir in FibonacciSphere)
        {
            if (Physics.SphereCastNonAlloc(transform.position, settings.ObstacleDetectionSphereRadius,transform.TransformDirection(dir), hits, settings.ObstacleDetectionRange, settings.ObstacleMask) == 0)
                return transform.TransformDirection(dir);
        }
        return transform.forward;
    }

    #endregion

    RaycastHit[] hits = new RaycastHit[1];
    Collider[] cols = new Collider[1];

    private bool CheckForCollision()
    {
        //if heading for collision, return true
        if (0 != Physics.SphereCastNonAlloc(transform.position, settings.ObstacleDetectionSphereRadius, transform.forward, hits, settings.ObstacleDetectionRange, Physics.AllLayers ^settings.ObstacleMask))
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


