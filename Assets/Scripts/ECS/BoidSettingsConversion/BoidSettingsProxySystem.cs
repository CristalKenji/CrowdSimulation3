using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
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
            ecb.AddComponent(entity, boidSettingsComponent);

            //ecb.SetComponent<BoidSettingsComponent>(entity, boidSettingsComponent);
            ecb.RemoveComponent<BoidSettingsProxyComponent>(entity);

        }).Schedule();
        commandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
