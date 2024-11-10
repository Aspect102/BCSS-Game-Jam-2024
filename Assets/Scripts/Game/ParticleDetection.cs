using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class ParticleDetection : MonoBehaviour
{
    [SerializeField] new ParticleSystem particleSystem;
    [SerializeField] GameObject player;
    

    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        particleSystem.trigger.AddCollider(player.GetComponentInChildren<CapsuleCollider>());
    }


    void Update()
    {
        
    }

    private void OnParticleTrigger()
    {
        player.GetComponent<PlayerCombat>().TakeDamage(-1);
    }
}
