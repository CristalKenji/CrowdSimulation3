using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BoidSettingsBlobAsset
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
    public float TargetWeight;
    public LayerMask TargetMask;


    public LayerMask BoidMask;
    public int ObstacleRays;
    public LayerMask ObstacleMask;
    public float ObstacleDetectionSphereRadius;
    public float ObstacleDetectionRange;
    public bool StripYAxis;
}
