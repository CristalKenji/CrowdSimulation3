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
    public int amountOfBoids = 100;
    [SerializeField]
    float spawnRadius = 10;

    internal BoidController[] boids;

    public BoidSettings settings;

    public int BoidCount
    {
        get
        {
            return boids.Length;
        }
    }

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
        SpawnBoids();
    }

    GameObject temp;
    public void SpawnBoids()
    {
        for (int boidCounter = 0; boidCounter < amountOfBoids; boidCounter++)
        {
            temp = Instantiate(boidPrefab, transform.position + Random.onUnitSphere * Random.Range(0f, spawnRadius), Random.rotation);
            if (settings.StripYAxis)
            {
                temp.transform.Translate(0, -temp.transform.position.y, 0, Space.World);
                temp.transform.rotation = Quaternion.identity;
                temp.transform.Rotate(0, Random.Range(0, 360), 0);
            }
        }
        boids = FindObjectsOfType<BoidController>();
    }
}
