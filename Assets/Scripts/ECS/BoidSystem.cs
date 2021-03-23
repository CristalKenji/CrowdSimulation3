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
        boidQuery = GetEntityQuery(typeof(Translation), typeof(Rotation), typeof(BoidMemoryComponent), ComponentType.ReadOnly<BoidSettingsComponent>());
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

        handle = this.Entities
        .ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, ref Rotation rotation, ref BoidMemoryComponent memory, in BoidSettingsComponent settings) =>
        {
            float3 vecToNeighbour;
            float3 ownForward = math.mul(rotation.Value, new float3(0, 0, 1));
            float3 velocity = ownForward * memory.speed;
            float3 cohesion = float3.zero;
            float3 separation = float3.zero;
            float3 alignment = ownForward;
            float3 neighbourForward = float3.zero;
            float3 neighbourPosition = float3.zero;
            int boidsConsidered = 0;

            BoidSettingsBlobAsset settingsCached = settings.boidSettingsBlobAssetReference.Value;
            for (int boidIndex = 0; boidIndex < boidCount; boidIndex++)
            {

                //only execute if close enough
                vecToNeighbour = translation.Value - boidPos[boidIndex].Value;
                if (math.lengthsq(vecToNeighbour) < settingsCached.AwarenessRadius * settingsCached.AwarenessRadius)
                {
                    //dont execute on self
                    if (boidIndex == entityInQueryIndex)
                    {
                        continue;
                    }
                    boidsConsidered++;
                    //initialize neighbour variables
                    neighbourPosition = boidPos[boidIndex].Value;
                    neighbourForward = math.mul(boidRot[boidIndex].Value, new float3(0, 0, 1));

                    if (settingsCached.StripYAxis)
                    {
                        alignment.y = 0;
                        neighbourPosition.y = 0;
                        neighbourForward.y = 0;
                        neighbourForward = math.normalize(neighbourForward);
                        vecToNeighbour.y = 0;
                    }

                    alignment += neighbourForward;
                    cohesion += neighbourPosition;
                    separation += math.normalize(vecToNeighbour) - (vecToNeighbour / settingsCached.AwarenessRadius);

                }
            }

            float3 cohesionSteeringDelta = float3.zero, alignmentSteeringDelta = float3.zero, separationSteeringDelta = float3.zero;

            alignment = math.normalize(alignment);
            if (boidsConsidered > 0)
            {
                cohesion = (cohesion / boidsConsidered) - translation.Value;
                separation /= boidsConsidered;
                cohesionSteeringDelta = math.normalize(cohesion) * math.clamp(math.length((math.normalize(cohesion) * settingsCached.MaxVelocity) - velocity), 0f, settingsCached.MaxSteerForce) * settingsCached.CohesionWeight;
                separationSteeringDelta = math.normalize(separation) * math.clamp(math.length((math.normalize(separation) * settingsCached.MaxVelocity) - velocity), 0f, settingsCached.MaxSteerForce) * settingsCached.SeparationWeight;

            }
            float3 direction;

            alignmentSteeringDelta = math.normalize(alignment) * math.clamp(math.length((math.normalize(alignment) * settingsCached.MaxVelocity) - velocity), 0f, settingsCached.MaxSteerForce) * settingsCached.AlignmentWeight;


            // calculate steering deltas, 134/155 in boid controller

            float3 combinedVelocityDelta = cohesionSteeringDelta + alignmentSteeringDelta + separationSteeringDelta;
            velocity += combinedVelocityDelta * deltaTime;
            direction = math.normalize(velocity);
            float speed = math.length(velocity);
            speed = math.clamp(speed, settingsCached.MinVelocity, settingsCached.MaxVelocity);
            velocity = direction * speed;
            memory.speed = speed;
            translation.Value += velocity * deltaTime;
            rotation.Value = quaternion.LookRotation(direction, math.cross(math.cross(direction, new float3(0, 0, 1)), direction));
        })
        .WithReadOnly(boidPos)
        .WithReadOnly(boidRot)
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
