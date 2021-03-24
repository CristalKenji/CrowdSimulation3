using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;

public class PathfindingSystem : SystemBase
{

    NativeArray<float3> grid;

    float3 offset = new float3(.5f, 0f, .5f);

    protected override void OnCreate()
    {

        grid = new NativeArray<float3>(3072, Allocator.Persistent);
    }
    protected override void OnUpdate()
    {
        int height, width;
        height = GridController.Height;
        width = GridController.Width;
        float3 offset = this.offset;
        NativeArray<float3> localGrid = this.grid;
        NativeArray<bool> unknownCoords = new NativeArray<bool>(width * height, Allocator.TempJob);
        JobHandle handle = Entities.ForEach((Entity entity, ref Translation translation, ref PathfindingComponent pathfinding) =>
       {
           int2 coords = new int2((translation.Value + offset).xz);
           pathfinding.dir = localGrid[coords.x + coords.y * width];
           if (pathfinding.dir.Equals(float3.zero))
           {
               unknownCoords[coords.x + coords.y * width] = true;
           }
       }).WithReadOnly(localGrid).WithNativeDisableParallelForRestriction(unknownCoords).ScheduleParallel(Dependency);
        handle.Complete();
        if (unknownCoords.Contains(true))
        {
            Debug.Log("Request update");
        }



        localGrid.Dispose();
        unknownCoords.Dispose();
    }

    private void GridToBetterGrid()
    {
        for (int i = 0; i < GridController.Height; i++)
        {
            for (int j = 0; j < GridController.Width; j++)
            {
                grid[i * GridController.Width + j] = GridController.Cell(i, j)?.Direction ?? float3.zero;
            }
        }
    }
}
