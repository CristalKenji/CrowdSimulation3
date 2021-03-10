using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject boidPrefab;
    [SerializeField]
    public int amountOfBoids = 100;
    [SerializeField]
    float spawnRadius = 10;
    [SerializeField]
    float spawnAngle = 360;
    [SerializeField]
    float spawnInterval = .5f;
    Driver driver;

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
    }

    // Start is called before the first frame update
    void Start()
    {
        driver ??= FindObjectOfType<Driver>();
        SpawnBoids();
        Driver.m_reportStartingPoints.AddListener(pleaseSubscribe);
    }

    void pleaseSubscribe()
    {
        Pathfinder.AddStartPoint(GridController.CellTransitPoint(new Vector2Int(Mathf.FloorToInt(transform.position.x + 0.5f), Mathf.FloorToInt(transform.position.z + 0.5f))));
        StartCoroutine("BoidSpawnCoroutine");

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
                temp.transform.Rotate(0, Random.Range(0, spawnAngle), 0);
            }
            temp.SetActive(false);
            temp.GetComponent<BoidController>().driver = driver;
            boidBuffer.Enqueue(temp);
        }
        boids = FindObjectsOfType<BoidController>();
    }


    IEnumerator BoidSpawnCoroutine()
    {
        while (true)
        {
            if (boidBuffer.Count > 0)
            {
                GameObject temp = boidBuffer.Dequeue();
                temp.transform.rotation = transform.rotation;
                temp.transform.Rotate(0, Random.Range(0, spawnAngle), 0);
                temp.transform.position = transform.position + temp.transform.forward * Random.Range(0, spawnRadius);

                temp.SetActive(true);
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    static Queue<GameObject> boidBuffer = new Queue<GameObject>();
    public static void QueueBoid(GameObject boid)
    {
        boid.SetActive(false);
        boidBuffer.Enqueue(boid);
    }
}
