using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class BoidGizmo
{
    static Color cohesionColor = Color.blue;
    static Color alignmentColor = Color.green;
    static Color separationColor = Color.red;
    static Color summedSteeringColor = Color.yellow;
    static Color obstacleRayColor = Color.white;
    static Color boidConnectionColor = Color.cyan;
    static Color CollidedColor = Color.red;


    [DrawGizmo(GizmoType.Active | GizmoType.Selected)]
    static void DrawCohesionDirection(BoidController src, GizmoType gizmoType)
    {
        Gizmos.color = cohesionColor;
        Gizmos.DrawLine(src.transform.position, src.transform.position + src.cohesion * src.settings.CohesionWeight);
    }

    [DrawGizmo(GizmoType.Active | GizmoType.Selected)]
    static void DrawAlignmentDirection(BoidController src, GizmoType gizmoType)
    {
        Gizmos.color = alignmentColor;
        Gizmos.DrawLine(src.transform.position, src.transform.position + src.alignment * src.settings.AlignmentWeight);
    }

    [DrawGizmo(GizmoType.Active | GizmoType.Selected)]
    static void DrawSeparationDirection(BoidController src, GizmoType gizmoType)
    {
        Gizmos.color = separationColor;
        Gizmos.DrawLine(src.transform.position, src.transform.position + src.separation * src.settings.SeparationWeight);
    }

    [DrawGizmo(GizmoType.Active | GizmoType.Selected)]
    static void DrawSummedSteeringDelta(BoidController src, GizmoType gizmoType)
    {
        Gizmos.color = summedSteeringColor;
        Gizmos.DrawLine(src.transform.position + src.transform.forward, src.transform.position + src.transform.forward + src.combinedVelocityDelta);
    }

    [DrawGizmo(GizmoType.Active | GizmoType.Selected)]
    static void DrawBoidsInRange(BoidController src, GizmoType gizmoType)
    {
        Gizmos.color = boidConnectionColor;
        foreach (Transform go in src.boidsToConsider)
        {
            Gizmos.DrawLine(src.transform.position, go.position);
        }
    }

    [DrawGizmo(GizmoType.Active | GizmoType.Selected)]
    static void DrawObstacleRays(BoidController src, GizmoType gizmoType)
    {
        Gizmos.color = obstacleRayColor;
        Gizmos.DrawLine(src.transform.position, src.transform.position + src.obstacleEvasion);
        Gizmos.DrawWireSphere(src.transform.position, src.settings.ObstacleDetectionSphereRadius);
    }

    [DrawGizmo(GizmoType.Active | GizmoType.Selected)]
    static void DrawBoidDetectionSphere(BoidController src, GizmoType gizmoType)
    {
        Gizmos.color = obstacleRayColor;
        Gizmos.DrawWireSphere(src.transform.position, src.settings.ObstacleDetectionRange);
    }

    [DrawGizmo(GizmoType.NotInSelectionHierarchy)]
    static void DrawCollidedBoid(BoidController src, GizmoType gizmoType)
    {
        if (src.collided)
        {
            Gizmos.color = CollidedColor;
            Gizmos.DrawSphere(src.transform.position, src.settings.ObstacleDetectionSphereRadius);
        }
    }
}