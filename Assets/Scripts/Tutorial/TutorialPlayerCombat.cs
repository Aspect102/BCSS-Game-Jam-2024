using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPlayerCombat : MonoBehaviour
{
    public Camera cam;

    public float attackDistance = 18f;
    public float attackDelay = 0.1f; // time between attack and sending out ray
    public float attackSpeed = 0.5f; // time between attacks
    public static float attackDamage = 1f;
    public LayerMask enemyLayer;

    bool attacking = false;
    bool readyToAttack = true;
    int attackCount;

    Animator animator;
    public const string SWING1 = "mop swing 1";
    public const string SWING2 = "mop swing 2";
    string currentAnimationState;


    void Awake()
    { 
        animator = GetComponentInChildren<Animator>();
        // audioSource = GetComponent<AudioSource>();
    }


    void Update()
    {
        // Repeat Inputs
        if(Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse0))
        { 
            Attack(); 
        }
    }


    public void Attack()
    {
        if (!readyToAttack || attacking) 
        {
            return;
        }

        readyToAttack = false;
        attacking = true;

        Invoke(nameof(ResetAttack), attackSpeed);
        Invoke(nameof(AttackRaycast), attackDelay);

        // audioSource.pitch = Random.Range(0.9f, 1.1f);
        // audioSource.PlayOneShot(swordSwing);

        if (attackCount == 0)
        {
            ChangeAnimationState(SWING1);
            attackCount++;
        }
        else
        {
            ChangeAnimationState(SWING2);
            attackCount = 0;
        }
    }


    void ResetAttack()
    {
        attacking = false;
        readyToAttack = true;
    }


    void AttackRaycast()
    {
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, attackDistance, enemyLayer))
        { 
            // HitTarget(hit.point);

            if(hit.transform.TryGetComponent<TutorialEnemyHurt>(out TutorialEnemyHurt enemyHurt))
            { 
                enemyHurt.TakeDamage(attackDamage);
            }
        } 
    }


    // void HitTarget(Vector3 pos)
    // {
    //     audioSource.pitch = 1;
    //     audioSource.PlayOneShot(hitSound);

    //     GameObject GO = Instantiate(hitEffect, pos, Quaternion.identity);
    //     Destroy(GO, 20);
    // }


    public void ChangeAnimationState(string newState) 
    {
        // STOP THE SAME ANIMATION FROM INTERRUPTING WITH ITSELF //
        if (currentAnimationState == newState) 
        {
            return;
        }
        
        // PLAY THE ANIMATION //
        currentAnimationState = newState;
        animator.CrossFadeInFixedTime(currentAnimationState, 0f);
    }

    // void SetAnimations()
    // {
    //     // If playerTransform is not attacking
    //     if(!attacking)
    //     {
    //         if(_PlayerVelocity.x == 0 &&_PlayerVelocity.z == 0)
    //         { ChangeAnimationState(IDLE); }
    //         else
    //         { ChangeAnimationState(WALK); }
    //     }
    // }

}

