using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHurt : MonoBehaviour
{
    public Camera cam;
    public PlayerUIManager playerUImanager;
    public PlayerCombat playerCombat;

    public float currentHealth;
    public int maxHealth;
    public float knockbackForce;
    public float timeTillCorpseCleanup;

    private Rigidbody[] _ragdollRigidbodies;

    public Transform audioContainer;

    void Awake()
    {
        currentHealth = maxHealth;

        // taken from OnCreationScript because prefabs cant reference the camera on their own
        cam = transform.GetComponent<OnCreationScript>().cam;

        audioContainer = transform.Find("AudioContainer");

        _ragdollRigidbodies = transform.Find("Hips").GetComponentsInChildren<Rigidbody>();
        DisableRagdoll();
    }

    private void DisableRagdoll()
    {
        foreach (var rigidbody in _ragdollRigidbodies)
        {
            rigidbody.isKinematic = true;
        }
    }

    public void EnableRagdoll()
    {
        // deactivate pathfinding, animations and scripts
        if (transform.TryGetComponent<NavMeshAgent>(out NavMeshAgent navMeshAgent))
        {
            navMeshAgent.enabled = false;
        }

        if (transform.TryGetComponent<RunEnemyController>(out RunEnemyController enemyRun))
        {
            enemyRun.enabled = false;
        }
        transform.GetComponent<Animator>().enabled = false;

        // deactivate collider (body part colliders used to collide with capsule but now configured not to, kept this line just in case)
        transform.GetComponent<CapsuleCollider>().enabled = false;

        foreach (var rigidbody in _ragdollRigidbodies)
        {
            rigidbody.isKinematic = false;
        }

        // knockback
        Rigidbody hipRB = transform.Find("Hips").GetComponent<Rigidbody>();
        var flatForward = new Vector3(cam.transform.forward.x, 0.4f, cam.transform.forward.z).normalized;
        hipRB.AddForce(flatForward * knockbackForce, ForceMode.VelocityChange);
    }


    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        { 
            Death(); 
        }
    }


    void Death()
    {
        switch (gameObject.GetComponent<CustomTags>().colourString)
        {
            case ("blue mat"):
                audioContainer.transform.Find("crash1").GetComponent<AudioSource>().enabled = true;
                break;
            case ("orange mat"):
                audioContainer.transform.Find("crash2").GetComponent<AudioSource>().enabled = true;
                break;
            case ("purple mat"):
                audioContainer.transform.Find("crash3").GetComponent<AudioSource>().enabled = true;
                break;
            case ("green mat"):
                audioContainer.transform.Find("crash4").GetComponent<AudioSource>().enabled = true;
                break;
            default:
                Debug.Log("shart alert");
                break;
        }
        
        playerUImanager = GameObject.FindWithTag("ServerScripts").GetComponent<PlayerUIManager>();
        playerUImanager.UpdateEnemyKilledIcons(gameObject);
        playerCombat = GameObject.FindWithTag("Player").GetComponent<PlayerCombat>();
        playerCombat.kills++;
        
        EnableRagdoll();
        Invoke(nameof(Cleanup), timeTillCorpseCleanup);
        
        GetComponentInChildren<ParticleSystem>().Play();
        ParticleSystem.EmissionModule emission = GetComponentInChildren<ParticleSystem>().emission;
        emission.enabled = true;
    }

    void Cleanup()
    {
        Destroy(gameObject);
    }
}

