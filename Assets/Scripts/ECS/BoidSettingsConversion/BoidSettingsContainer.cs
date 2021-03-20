using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSettingsContainer : MonoBehaviour
{
    [SerializeField]
    private BoidSettings boidSettings;
    public BoidSettings BoidSettings => boidSettings;
}
