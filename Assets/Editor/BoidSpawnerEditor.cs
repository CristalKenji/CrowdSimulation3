using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BoidSpawner))]
public class BoidSpawnerEditor : Editor
{

    float boidsToSpawnOnButton;

    public override void OnInspectorGUI()
    {
        BoidSpawner bs = (BoidSpawner) target;
        base.OnInspectorGUI();

        if(Application.isPlaying){
            bs.amountOfBoids = EditorGUILayout.IntField("Amount of boids to spawn", bs.amountOfBoids);
            EditorGUILayout.IntField("Boids spawned", bs.BoidCount);
            if (GUILayout.Button("Spawn Boids"))
            {
                bs.SpawnBoids();
            }
        }

    }
}
