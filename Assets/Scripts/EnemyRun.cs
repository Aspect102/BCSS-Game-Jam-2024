using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRun : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask groundLayer, playerLayer;

    public float attackSpeed; // time between attacks
    bool alreadyAttacked;

    public float attackRange;
    bool playerInRange;

    Animator animator;
    public const string RUN = "stickman run";
    public const string PUNCH = "stickman punch";
    string currentAnimationState;


    private void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();

        animator = GetComponentInChildren<Animator>();
    }


    private void Update() {
        // playerInRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);
        float dist = Vector3.Distance(player.position, transform.position);
        if (dist <= attackRange) // playerInRange
        {
            ChangeAnimationState(PUNCH);
            AttackPlayer();
        }
        else
        {
            ChangeAnimationState(RUN);
            ChasePlayer();
        }
    }


    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }


    private void AttackPlayer()
    {
        // ATTACK HERE

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), attackSpeed);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
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


    // private void OnDrawGizmosSelected() {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(transform.position, attackRange);    
    // }
}
