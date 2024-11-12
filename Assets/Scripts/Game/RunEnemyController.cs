using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RunEnemyController : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform playerTransform;
    public LayerMask groundLayer, playerLayer;

    public static float attackSpeed = 1.5f; // time between attacks
    public static float attackDamage = 15;

    public float attackRange;

    Animator animator;
    public const string RUN = "stickman run";
    public const string PUNCH = "stickman punch";
    public const string IDLE = "stickman idle";
    string currentAnimationState;


    private void Awake()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();

        animator = GetComponentInChildren<Animator>();
    }


    private void Update()
    {
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(playerTransform.position, path);

        ChasePlayer(path);

        transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));

        // playerInRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        float dist = Vector3.Distance(playerTransform.position, transform.position);

        if (path.status != NavMeshPathStatus.PathPartial)   // check if playerTransform is reachable
        {
            if (dist <= attackRange)
            {
                ChangeAnimationState(PUNCH);
            }
            else
            {
                ChangeAnimationState(RUN);
            }

        }
        else
        {
            ChangeAnimationState(IDLE);
        }
    }


    private void ChasePlayer(NavMeshPath path)
    {
        if (path.status != NavMeshPathStatus.PathPartial)   // check if playerTransform is reachable
        {
            agent.SetDestination(playerTransform.position);
        }
        else
        {
            agent.SetDestination(transform.position);   // stay at current position
        }
    }

    private IEnumerator AttackPlayer()
    {
        GameObject player = gameObject.GetComponent<OnCreationScript>().player;
        PlayerController playercontroller = player.GetComponent<PlayerController>();
        
        if (playercontroller.isGrounded)
        {
            PlayerCombat playerCombat = player.GetComponent<PlayerCombat>();
            playerCombat.TakeDamage(attackDamage);
            yield return new WaitForSeconds(attackSpeed);
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
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
        {
            StartCoroutine(AttackPlayer());
        }
    }


    // private void OnDrawGizmosSelected() {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(transform.position, attackRange);    
    // }
}
