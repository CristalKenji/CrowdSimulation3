using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct BoidSettingsComponent : IComponentData
{
    public BlobAssetReference<BoidSettingsBlobAsset> boidSettingsBlobAssetReference;
}
