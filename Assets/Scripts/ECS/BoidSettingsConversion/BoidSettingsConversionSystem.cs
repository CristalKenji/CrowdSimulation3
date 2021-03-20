using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(GameObjectConversionGroup))]
public class BoidSettingsConversionSystem : GameObjectConversionSystem
{

    public static BlobAssetReference<BoidSettingsBlobAsset> BoidSettingsAssetReference;
    protected override void OnUpdate()
    {
        this.Entities.ForEach((BoidSettingsContainer container) =>
        {
            using (BlobBuilder blobBuilder = new BlobBuilder(Unity.Collections.Allocator.Temp))
            {
                ref BoidSettingsBlobAsset blobAsset = ref blobBuilder.ConstructRoot<BoidSettingsBlobAsset>();
                blobAsset.AlignmentWeight = container.BoidSettings.AlignmentWeight;
                blobAsset.AwarenessRadius = container.BoidSettings.AwarenessRadius;
                blobAsset.BoidMask = container.BoidSettings.BoidMask;
                blobAsset.CohesionWeight = container.BoidSettings.CohesionWeight;
                blobAsset.MaxSteerForce = container.BoidSettings.MaxSteerForce;
                blobAsset.MaxVelocity = container.BoidSettings.MaxVelocity;
                blobAsset.MinVelocity = container.BoidSettings.MinVelocity;
                blobAsset.ObstacleAvoidanceWeight = container.BoidSettings.ObstacleAvoidanceWeight;
                blobAsset.ObstacleDetectionRange = container.BoidSettings.ObstacleDetectionRange;
                blobAsset.ObstacleDetectionSphereRadius = container.BoidSettings.ObstacleDetectionSphereRadius;
                blobAsset.ObstacleMask = container.BoidSettings.ObstacleMask;
                blobAsset.ObstacleRays = container.BoidSettings.ObstacleRays;
                blobAsset.SeparationWeight = container.BoidSettings.SeparationWeight;
                blobAsset.StripYAxis = container.BoidSettings.StripYAxis;
                blobAsset.TargetMask = container.BoidSettings.TargetMask;
                blobAsset.TargetWeight = container.BoidSettings.TargetWeight;
                blobAsset.UseOptimizedRules = container.BoidSettings.UseOptimizedRules;

                BoidSettingsAssetReference = blobBuilder.CreateBlobAssetReference<BoidSettingsBlobAsset>(Unity.Collections.Allocator.Persistent);
            }
        });
        Debug.Log($"{BoidSettingsAssetReference.Value.AlignmentWeight.ToString()}");
    }
}
