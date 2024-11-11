using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RangeEnemyController : MonoBehaviour
{
    Transform playerTransform;
    EnemyHurt enemyHurt;

    public float health = 1;
    public static float minAttackSpeed = 2;
    public static float maxAttackSpeed = 3; // time between attacks
    public Transform spawnPoint;
    public GameObject bulletPrefab;

    Animator animator;
    // public const string THROW = "stickman ";
    public const string IDLE = "stickman idle";
    string currentAnimationState;


    private void Awake()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        spawnPoint = transform.Find("SpawnPoint");
    
        enemyHurt = gameObject.GetComponent<EnemyHurt>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start() 
    {
        StartCoroutine(AttackPlayer());
    }


    private void Update()
    {
        health = enemyHurt.currentHealth;
        transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));
    }


    private IEnumerator AttackPlayer()
    {
        while (health > 0)
        {
            GameObject prefab = Instantiate(bulletPrefab, spawnPoint.position, Quaternion.identity);
            Material enemyMaterial = transform.Find("body").GetComponent<SkinnedMeshRenderer>().material; 
            prefab.GetComponent<SkinnedMeshRenderer>().material = enemyMaterial;

            yield return new WaitForSeconds(Random.Range(minAttackSpeed, maxAttackSpeed));
        }
    }


    public void ChangeAnimationState(string newState)
    {
        // STOP THE SAME ANIMATION FROM INTERRUPTING WITH ITSELF //
        if (currentAnimationState == newState)
        {
            return;
        }

        // PLAY THE ANIMATION //

        currentAnimationState = newState;
        animator.CrossFadeInFixedTime(currentAnimationState, 0.3f);
    }
}
