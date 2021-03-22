using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Jobs;

/// supposed to handle the boid movement in ECS
public class BoidSystem : SystemBase
{
    private EntityQuery boidQuery;



    protected override void OnCreate()
    {
        base.OnCreate();
        boidQuery = GetEntityQuery(typeof(Translation), typeof(Rotation), ComponentType.ReadOnly<BoidSettingsComponent>());
    }

    JobHandle handle;
    protected override void OnUpdate()
    {
        NativeArray<Translation> boidPos;
        NativeArray<Rotation> boidRot;
        EntityQuery localQuery = boidQuery;
        boidPos = localQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        boidRot = localQuery.ToComponentDataArray<Rotation>(Allocator.TempJob);
        float deltaTime = UnityEngine.Time.deltaTime;
        int boidCount = localQuery.CalculateEntityCount();


        if (!BoidSettingsConversionSystem.BoidSettingsAssetReference.IsCreated)
        {
            Debug.Log("boidsettingsassetreference not created");
            return;
        }

        handle = this.Entities.WithAll<BoidSettingsComponent, Translation, Rotation>()

        .ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, ref Rotation rotation, ref BoidSettingsComponent settings) =>
        {
            float3 vecToNeighbour;
            float3 cohesion = float3.zero, separation = float3.zero;
            float3 forward = math.mul(rotation.Value, new float3(0, 0, 1));
            BoidSettingsBlobAsset settingsCached = settings.boidSettingsBlobAssetReference.Value;
            for (int boidIndex = 0; boidIndex < boidCount; boidIndex++)
            {
                //dont execute on self
                if (boidIndex != entityInQueryIndex)
                {
                    //only execute if close enough
                    vecToNeighbour = math.normalize(translation.Value - boidPos[boidIndex].Value);
                    if (math.lengthsq(vecToNeighbour) > settings.boidSettingsBlobAssetReference.Value.AwarenessRadius * settings.boidSettingsBlobAssetReference.Value.AwarenessRadius)
                    {

                    }
                }
            }
            translation.Value += forward * deltaTime;
        })
        .WithReadOnly(boidPos)
        .ScheduleParallel(Dependency);
        handle.Complete();

        boidPos.Dispose();
        boidRot.Dispose();



    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
