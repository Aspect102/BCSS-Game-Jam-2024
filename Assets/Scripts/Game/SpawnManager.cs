using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using Random = UnityEngine.Random;


public struct GameObjBool
{
    public GameObject GameObject;
    public bool CanSpawn;

    public GameObjBool(GameObject gameObject, bool canSpawn)
    {
        GameObject = gameObject;
        CanSpawn = canSpawn;
    }
}


public class SpawnManager : MonoBehaviour
{
    public RoundManager roundManager;
    public PlayerUIManager playerUIManager;
    public GameObject runEnemyPrefab;
    public GameObject rangedEnemyPrefab;

    public GameObject[] mapBorders = new GameObject[4];
    GameObjBool[] possibleRangedSpawns;
    int numberPossibleSpawns;

    public Material[] materials = new Material[4];  

    public float MinSpawnDelay; // measured in seconds
    public float MaxSpawnDelay;
    public int canSpawnDetectionRadius = 10;

    private (float, float) xRange; //tuple that stores the lower bound to the upper bound
    private (float, float) zRange;

    private float randX;
    private float randZ;
    private Vector3 spawnVector;


    void Awake()
    {
        xRange = (Mathf.Min(mapBorders[0].transform.position.x, mapBorders[1].transform.position.x, mapBorders[2].transform.position.x, mapBorders[3].transform.position.x),
            Mathf.Max(mapBorders[0].transform.position.x, mapBorders[1].transform.position.x, mapBorders[2].transform.position.x, mapBorders[3].transform.position.x));

        zRange = (Mathf.Min(mapBorders[0].transform.position.z, mapBorders[1].transform.position.z, mapBorders[2].transform.position.z, mapBorders[3].transform.position.z),
            Mathf.Max(mapBorders[0].transform.position.z, mapBorders[1].transform.position.z, mapBorders[2].transform.position.z, mapBorders[3].transform.position.z));
    
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        numberPossibleSpawns = spawnPoints.Length;
        possibleRangedSpawns = new GameObjBool[numberPossibleSpawns];


        for (int i = 0; i < numberPossibleSpawns; i++)
        {
            possibleRangedSpawns[i] = new GameObjBool(spawnPoints[i], true);
        }
    }
    

    void SetPrefabProperties(Material[] materials, GameObject prefab)
    {
        #region Assign new random material

        Material randomMaterial = materials[Random.Range(0, 4)];
        prefab.GetComponentInChildren<SkinnedMeshRenderer>().material = randomMaterial;

        #endregion

        #region Set SplatterController

        Gradient gradient = new Gradient();
        var colors = new GradientColorKey[2];
        colors[0] = new GradientColorKey(randomMaterial.color, 0.0f);
        colors[1] = new GradientColorKey(randomMaterial.GetColor("_SpecColor"), 1.0f);

        var alphas = new GradientAlphaKey[2];
        alphas[0] = new GradientAlphaKey(1.0f, 1.0f);
        alphas[1] = new GradientAlphaKey(1.0f, 1.0f);

        gradient.SetKeys(colors, alphas);
        prefab.GetComponentInChildren<SplatterController>().particleGradient = gradient;

        #endregion

        #region Set Particle System

        ParticleSystem.MainModule startColor = prefab.GetComponentInChildren<ParticleSystem>().main;
        startColor.startColor = gradient;

        #endregion

        #region Add colour tags based on colour for top text enemy tracking

        prefab.GetComponent<CustomTags>().colourMaterial = randomMaterial;

        #endregion
    }


    public IEnumerator StartSpawning(int enemyRoundCount)
    {
        float percentageRanged = 0.125f;
        int numberOfRangedEnemies = (int)Math.Ceiling(enemyRoundCount * percentageRanged);
        int numberOfRunEnemies = enemyRoundCount - numberOfRangedEnemies;

        Debug.Log("run enemies:" + numberOfRunEnemies);
        Debug.Log("ranged enemies:" + numberOfRangedEnemies);

        for (int i = 0; i < numberOfRunEnemies; i++)
        {
            randX = Random.Range(xRange.Item1, xRange.Item2);
            randZ = Random.Range(zRange.Item1, zRange.Item2);
            spawnVector = new Vector3(randX, 0, randZ); // y = 0 implies that it will always spawn on the floor, no elevation sorry (yet?)

            if (Physics.CheckSphere(spawnVector, canSpawnDetectionRadius))
            {
                GameObject newEnemy = Instantiate(runEnemyPrefab, spawnVector, Quaternion.identity);
                SetPrefabProperties(materials, newEnemy);
            };

            yield return new WaitForSeconds(Random.Range(MinSpawnDelay, MaxSpawnDelay));
            enemyRoundCount--;
            roundManager.enemyLeftToSpawn--;
        }

        // create temporary struct to modify for this round only
        GameObjBool[] tempPossibleRangedSpawns = possibleRangedSpawns;

        for (int i = 0; i < numberOfRangedEnemies; i++)
        {
            bool notFoundLocation = true;

            while (notFoundLocation)
            {
                int chosenLocation = Random.Range(0, numberPossibleSpawns);
                if (tempPossibleRangedSpawns[chosenLocation].CanSpawn == true)
                {
                    notFoundLocation = false;
                    tempPossibleRangedSpawns[chosenLocation].CanSpawn = false;

                    spawnVector = tempPossibleRangedSpawns[chosenLocation].GameObject.transform.position;

                    GameObject newEnemy = Instantiate(rangedEnemyPrefab, spawnVector, Quaternion.identity);
                    SetPrefabProperties(materials, newEnemy);

                    yield return new WaitForSeconds(Random.Range(MinSpawnDelay, MaxSpawnDelay));
                    enemyRoundCount--;
                    roundManager.enemyLeftToSpawn--;
                }
            }
        }

        while (true)
        {
            if (!GameObject.FindGameObjectWithTag("Enemy"))
            {
                playerUIManager.ShowCards();
                yield break;
            }
            yield return null;
        }
    }
}
