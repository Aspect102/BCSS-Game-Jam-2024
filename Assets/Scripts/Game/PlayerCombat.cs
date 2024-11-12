using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public PlayerController playerController;

    public Camera cam;
    public RoundManager roundManager;
    public PlayerUIManager playerUIManager;

    public float playerHealth;
    public float playerMaxHealth;
    public bool damageBufferActive = false;
    public float bufferResetTime;

    public int kills;
    public float attackDistance = 18f;
    public float attackDelay = 0.1f; // time between attack and sending out ray
    public float attackSpeed = 0.5f; // time between attacks
    public static float attackDamage = 1f;
    public LayerMask enemyLayer;

    bool attacking = false;
    bool readyToAttack = true;
    int attackCount;

    bool isGrounded;

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
        isGrounded = playerController.isGrounded;

        // Repeat Inputs
        if(Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse0))
        { 
            Attack(); 
        }
    }
    

    public void TakeDamage(float amount)
    {
        if (!damageBufferActive || amount < 0) // only deal damage if buffer not active and amount is negative (healing)
        {
            damageBufferActive = true;
            StartCoroutine(ResetDamageBuffer());

            var newHealth = playerHealth - amount;
            playerHealth = Mathf.Clamp(newHealth, 0, playerMaxHealth);
            playerUIManager.VignetteCheck(playerHealth, playerMaxHealth);

            if (playerHealth == 0)
            {
                roundManager.RoundFail();
            }
        }
    }


    public IEnumerator ResetDamageBuffer()
    {
        yield return new WaitForSeconds(bufferResetTime);
        damageBufferActive = false;
        yield break;
    } 


    public void ResetHealth()
    {
        playerHealth = playerMaxHealth;
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

            if(hit.transform.TryGetComponent<EnemyHurt>(out EnemyHurt enemyHurt))
            { 
                enemyHurt.TakeDamage(attackDamage);
            }
        } 
    }


    public IEnumerator checkEnemyBeneathSlam()
    {
        Transform groundCheck = playerController.groundCheck;
        while(!isGrounded)
        {
            // Debug.Log("trying");
            // if(Physics.Raycast(groundCheck.position, -Vector3.up, out RaycastHit hit, 5f, enemyLayer))
            // {
            //     Debug.Log("hits");
            //     if(hit.transform.TryGetComponent<EnemyHurt>(out EnemyHurt enemyHurt))
            //     { 
            //         enemyHurt.TakeDamage(attackDamage);
            //     }
            // }

            Collider[] hitColliders = Physics.OverlapSphere(groundCheck.position, playerController.groundDistance + 1, enemyLayer); // ground check

            foreach (var hitCollider in hitColliders)
            {
                hitCollider.transform.TryGetComponent<EnemyHurt>(out EnemyHurt enemyHurt);
                enemyHurt.TakeDamage(attackDamage);
            }

            yield return null;
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
