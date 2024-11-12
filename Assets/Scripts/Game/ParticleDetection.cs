using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ParticleDetection : MonoBehaviour
{
    [SerializeField] new ParticleSystem particleSystem;
    [SerializeField] GameObject player;

    public Scene currentScene;
    string sceneName;
    

    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        particleSystem.trigger.AddCollider(player.GetComponentInChildren<CapsuleCollider>());

        currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;
    }


    void Start()
    {
        
    }

    private void OnParticleTrigger()
    {
        if (sceneName == "Game")
        {
            player.GetComponent<PlayerCombat>().TakeDamage(-0.5f);
        }
    }
}
