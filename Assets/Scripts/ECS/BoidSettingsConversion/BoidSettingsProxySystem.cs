using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

//exchanges the boidsettingsproxycomponent with the boidsettingscomponent
public class BoidSettingsProxySystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem commandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        var ecb = commandBufferSystem.CreateCommandBuffer();
        EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;
        BlobAssetReference<BoidSettingsBlobAsset> temp = BoidSettingsConversionSystem.BoidSettingsAssetReference;

        Entities.WithAll<BoidSettingsProxyComponent>().ForEach((Entity entity) =>
        {

            BoidSettingsComponent boidSettingsComponent = new BoidSettingsComponent
            {
                boidSettingsBlobAssetReference = temp
            };
            BoidMemoryComponent memoryComponent = new BoidMemoryComponent
            {
                speed = (temp.Value.MinVelocity + temp.Value.MaxVelocity) / 2,
                pathfindingVector = float3.zero
            };
            ecb.AddComponent(entity, boidSettingsComponent);
            ecb.AddComponent(entity, memoryComponent);
            ecb.RemoveComponent<BoidSettingsProxyComponent>(entity);

        }).Schedule();
        commandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
