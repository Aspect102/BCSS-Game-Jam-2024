using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    public RoundManager roundmanager;
    public GameObject enemyPrefab;
    public GameObject[] mapBorders = new GameObject[4];
    public Material[] materials = new Material[4];  
    public int MinSpawnDelay = 0; // measured in seconds
    public int MaxSpawnDelay = 5;
    public int canSpawnDetectionRadius = 5;

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
        while (enemyRoundCount > 0)
        {
            randX = Random.Range(xRange.Item1, xRange.Item2);
            randZ = Random.Range(zRange.Item1, zRange.Item2);
            spawnVector = new Vector3(randX, 0, randZ); // y = 0 implies that it will always spawn on the floor, no elevation sorry (yet?)

            if (Physics.CheckSphere(spawnVector, canSpawnDetectionRadius))
            {
                GameObject newEnemy = Instantiate(enemyPrefab, spawnVector, Quaternion.identity);
                SetPrefabProperties(materials, newEnemy);
            };

            yield return new WaitForSeconds(Random.Range(MinSpawnDelay, MaxSpawnDelay));
            enemyRoundCount--;
            roundmanager.enemyLeftToSpawn--;
        }

        while (true)
        {
            if (!GameObject.FindGameObjectWithTag("Enemy"))
            {
                roundmanager.RoundRestart();
                yield break;
            }
            yield return null;
        }
    }
}
