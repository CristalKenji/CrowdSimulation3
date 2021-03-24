using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct PathfindingComponent : IComponentData
{
    public float3 dir;
}
