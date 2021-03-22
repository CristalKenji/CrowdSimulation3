using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Physics;


/// creates pure ecs entities of boids, works alright, but shading is weird
public class BoidSpawnerECS : MonoBehaviour
{
    private EntityManager entityManager;

    [SerializeField]
    private Mesh boidMesh;
    [SerializeField]
    private UnityEngine.Material boidMaterial;
    [SerializeField]
    private GameObject boidECSPrefab;
    private EntityArchetype boidArchetype;

    [SerializeField]
    private bool usePrefab;
    [SerializeField]
    private int boidsToSpawn;
    private Unity.Mathematics.Random random;


    private void Start()
    {
        random = new Unity.Mathematics.Random(123);

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        boidArchetype = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(Rotation),
            typeof(RenderMesh),
            typeof(RenderBounds),
            typeof(LocalToWorld),
            typeof(BoidSettingsComponent)
        //typeof(PhysicsCollider)
        );

    }

    public void Update()
    {
        if (boidsToSpawn > 0)
        {
            if (usePrefab)
                SpawnPrefab();
            else
                SpawnPureECS();
            boidsToSpawn--;
        }
    }

    private void SpawnPrefab()
    {
        GameObject.Instantiate(boidECSPrefab, transform.position, quaternion.RotateY(random.NextFloat(360)));
    }

    private void SpawnPureECS()
    {
        Entity entity = entityManager.CreateEntity(boidArchetype);

        entityManager.AddComponentData(entity, new Translation { Value = new float3(transform.position) });
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
