using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct BoidMemoryComponent : IComponentData
{
    public float speed;
    public float3 pathfindingVector;
}
