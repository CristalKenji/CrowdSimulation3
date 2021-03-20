using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

public class BoidSystem : SystemBase
{
    private EntityQuery boidQuery;

    NativeArray<Translation> boidPos;
    NativeArray<Rotation> boidRot;
    protected override void OnCreate()
    {
        base.OnCreate();
        boidQuery = GetEntityQuery(typeof(Translation), typeof(Rotation), ComponentType.ReadOnly<BoidSettingsComponent>());
    }

    protected override void OnUpdate()
    {
        boidPos = boidQuery.ToComponentDataArray<Translation>(Allocator.Temp);
        boidRot = boidQuery.ToComponentDataArray<Rotation>(Allocator.Temp);

        if (!BoidSettingsConversionSystem.BoidSettingsAssetReference.IsCreated)
        {
            return;
        }
        this.Entities.WithAll<BoidSettingsComponent, Translation, Rotation>()
        .ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, ref Rotation rotation, in BoidSettingsComponent settings) =>
        {

        })
        .ScheduleParallel();
    }
}
