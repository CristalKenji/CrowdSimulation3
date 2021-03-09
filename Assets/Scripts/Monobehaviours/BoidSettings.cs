using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class BoidSettings : ScriptableObject
{
    public float AwarenessRadius;

    public float MinVelocity;
    public float MaxVelocity;
    public float MaxSteerForce;
    public bool UseOptimizedRules;

    public float CohesionWeight;
    public float AlignmentWeight;
    public float SeparationWeight;
    public float ObstacleAvoidanceWeight;
    public LayerMask BoidMask;
    public int ObstacleRays;
    public LayerMask ObstacleMask;
    public float ObstacleDetectionSphereRadius;
    public float ObstacleDetectionRange;
    public Material boidMaterialDefault;
    public Material boidMaterialHeadingForCollision;
    public Material boidMaterialEscaped;
    public bool StripYAxis;
}
