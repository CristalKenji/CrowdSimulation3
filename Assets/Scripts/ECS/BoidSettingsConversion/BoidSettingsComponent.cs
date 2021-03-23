using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

/// Component holding a reference to a boidsettingblobasset, aka boid setting struct
public struct BoidSettingsComponent : IComponentData
{
    public BlobAssetReference<BoidSettingsBlobAsset> boidSettingsBlobAssetReference;
}
