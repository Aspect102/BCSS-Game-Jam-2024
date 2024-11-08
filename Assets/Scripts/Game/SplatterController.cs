using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SplatterController : MonoBehaviour
{
    public ParticleSystem splatterParticles;
    public ParticleDecalPool splatDecalPool;
    public Gradient particleGradient;

    List<ParticleCollisionEvent> collisionEvents;

    // Start is called before the first frame update
    void Start()
    {
        collisionEvents = new List<ParticleCollisionEvent>();
        splatDecalPool = transform.parent.transform.GetComponent<OnCreationScript>().splatterDecalParticles; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnParticleCollision(GameObject other)
    {
        ParticlePhysicsExtensions.GetCollisionEvents(splatterParticles, other, collisionEvents);
        {
            for (int i = 0; i < collisionEvents.Count; i++)
            {
                splatDecalPool.ParticleHit(collisionEvents[i], particleGradient);
            }
        }
    }

}
