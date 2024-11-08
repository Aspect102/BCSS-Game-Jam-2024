using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject[] mapBorders = new GameObject[3];
    public int MinSpawnDelay = 0; // measured in seconds
    public int MaxSpawnDelay = 5;
    public int canSpawnDetectionRadius = 5;
    public bool haltSpawning = false;

    private (float, float) xRange; //tuple that stores the lower bound to the upper bound
    private (float, float) zRange;

    private float randX;
    private float randZ;
    private bool canSpawn;
    private Vector3 spawnVector;

    // Start is called before the first frame update
    void Start()
    {
        xRange = (Mathf.Min(mapBorders[0].transform.position.x, mapBorders[1].transform.position.x, mapBorders[2].transform.position.x, mapBorders[3].transform.position.x),
            Mathf.Max(mapBorders[0].transform.position.x, mapBorders[1].transform.position.x, mapBorders[2].transform.position.x, mapBorders[3].transform.position.x));

        zRange = (Mathf.Min(mapBorders[0].transform.position.z, mapBorders[1].transform.position.z, mapBorders[2].transform.position.z, mapBorders[3].transform.position.z),
            Mathf.Max(mapBorders[0].transform.position.z, mapBorders[1].transform.position.z, mapBorders[2].transform.position.z, mapBorders[3].transform.position.z));

        StartCoroutine(nameof(StartSpawning));
    }

    // note: not Update() func
    IEnumerator StartSpawning()
    {
        while (!haltSpawning)
        {
            randX = Random.Range(xRange.Item1, xRange.Item2);
            randZ = Random.Range(zRange.Item1, zRange.Item2);
            spawnVector = new Vector3(randX, 0, randZ); // y = 0 implies that it will always spawn on the floor, no elevation sorry (yet?)

            if (canSpawn = Physics.CheckSphere(spawnVector, canSpawnDetectionRadius))
            {
                Instantiate(enemyPrefab, spawnVector, Quaternion.identity);
            };

            yield return new WaitForSeconds(Random.Range(MinSpawnDelay, MaxSpawnDelay));
        }
    }
}
