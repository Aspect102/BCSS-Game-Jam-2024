using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCreationScript : MonoBehaviour
{
    public Camera cam;
    public GameObject player;
    public ParticleDecalPool splatterDecalParticles;

    void Awake() 
    {
        cam = FindObjectOfType<Camera>();
        player = GameObject.FindWithTag("Player");
        splatterDecalParticles = GameObject.FindWithTag("SplatterDecalParticles").GetComponent<ParticleDecalPool>();
    }

//     void Start() {
//         playerTransform = findg
//     }
}
