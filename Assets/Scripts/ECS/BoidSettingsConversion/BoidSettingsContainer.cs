using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Container for the boidsettings asset, needs to be on a gameobject in a subscene
public class BoidSettingsContainer : MonoBehaviour
{
    [SerializeField]
    private BoidSettings boidSettings;
    public BoidSettings BoidSettings => boidSettings;
}
