using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Physics;

public class BoidSpawnerECS : MonoBehaviour
{
    private EntityManager entityManager;

    [SerializeField]
    private Mesh boidMesh;
    [SerializeField]
    private UnityEngine.Material boidMaterial;
    private Unity.Mathematics.Random random;

    private void Start()
    {
        random = new Unity.Mathematics.Random(123);

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityArchetype boidArchetype = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(Rotation),
            typeof(RenderMesh),
            typeof(RenderBounds),
            typeof(LocalToWorld),
            typeof(BoidSettingsComponent)
        //typeof(PhysicsCollider)
        );
        Entity entity = entityManager.CreateEntity(boidArchetype);

        entityManager.AddComponentData(entity, new Translation { Value = new float3(0f, 0f, 0f) });
        entityManager.AddComponentData(entity, new Rotation { Value = quaternion.RotateY(random.NextFloat(360)) });
        entityManager.AddSharedComponentData(entity, new RenderMesh
        {
            mesh = boidMesh,
            material = boidMaterial
        });
        BoidSettingsComponent boidSettingsComponent = new BoidSettingsComponent
        {
            boidSettingsBlobAssetReference = BoidSettingsConversionSystem.BoidSettingsAssetReference
        };
        entityManager.AddComponentData(entity, boidSettingsComponent);

    }
}
