using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSpawner : MonoBehaviour
{
    private static BoidSpawner instance;
    public static BoidSpawner Instance
    {
        get { return instance; }
    }


    [SerializeField]
    GameObject boidPrefab;
    [SerializeField]
    int amountOfBoids = 100;
    [SerializeField]
    float spawnRadius = 10;

    internal BoidController[] boids;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int boidCounter = 0; boidCounter < amountOfBoids; boidCounter++)
        {
            Instantiate(boidPrefab, transform.position + Random.onUnitSphere * Random.Range(0f, spawnRadius), Random.rotation);
        }
        boids = FindObjectsOfType<BoidController>();
    }
}
