using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHurt : MonoBehaviour
{
    public Camera cam;

    int currentHealth;
    public int maxHealth;

    private Rigidbody[] _ragdollRigidbodies;


    // Start is called before the first frame update
    void Start()
    {
        
    }


    void Awake()
    {
        currentHealth = maxHealth;

        // taken from OnCreationScript because prefabs cant reference the camera on their own
        cam = transform.GetComponent<OnCreationScript>().cam;

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

    private void EnableRagdoll()
    {
        // deactivate pathfinding, animations and scripts
        transform.GetComponent<NavMeshAgent>().enabled = false;
        transform.GetComponent<EnemyRun>().enabled = false;
        transform.GetComponent<Animator>().enabled = false;

        // deactivate collider (body part colliders used to collide with capsule but now configured not to, kept this line just in case)
        transform.GetComponent<CapsuleCollider>().enabled = false;

        foreach (var rigidbody in _ragdollRigidbodies)
        {
            rigidbody.isKinematic = false;
        }

        // knockback
        Rigidbody hipRB = transform.Find("Hips").GetComponent<Rigidbody>();
        var flatForward = new Vector3(cam.transform.forward.x, 0.2f, cam.transform.forward.z).normalized;
        hipRB.AddForce(flatForward * 80f, ForceMode.VelocityChange);

        
    }


    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if(currentHealth <= 0)
        { 
            Death(); 
        }
    }


    void Death()
    {
        // Death function
        // TEMPORARY: Destroy Object

        EnableRagdoll();
        
        GetComponentInChildren<ParticleSystem>().Play();
        ParticleSystem.EmissionModule emission = GetComponentInChildren<ParticleSystem>().emission;
        emission.enabled = true;
    }
}
