using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDecalPool : MonoBehaviour
{
    public int maxDecals = 5000;
    public float decalSizeMin = 0.5f;
    public float decalSizeMax = 1.5f;

    private ParticleSystem decalParticleSystem; // SplatterDecalParticles game object
    private int particleDecalDataIndex;
    private ParticleDecalData[] particleData; // create a list of splatter particle details, to be displayed
    private ParticleSystem.Particle[] particles; // create a list of particles, Particle is a class of a particle system

    // Start is called before the first frame update
    void Start()
    {
        decalParticleSystem = GetComponent<ParticleSystem>();

        particles = new ParticleSystem.Particle[maxDecals];
        particleData = new ParticleDecalData[maxDecals];
        for (int i = 0; i < maxDecals; i++) // initalise list of particle decal data for each particle
        {
            particleData[i] = new ParticleDecalData();
        }
    }


    void LateUpdate() 
    {
        // set each splatter decals' data
        for (int i = 0; i < particleData.Length; i++)
        {
            particles[i].position = particleData[i].position;
            particles[i].rotation3D = particleData[i].rotation;
            particles[i].startSize = particleData[i].size;
            particles[i].startColor = particleData[i].color;
        }

        decalParticleSystem.SetParticles(particles, particles.Length); // SetParticles is a class of a particle system    
    }


    // Particle hits ground and calls this function
    public void ParticleHit(ParticleCollisionEvent particleCollisionEvent, Gradient colorGradient)
    {
        SetParticleData(particleCollisionEvent, colorGradient);
        // DisplayParticles();
    }


    void SetParticleData(ParticleCollisionEvent particleCollisionEvent, Gradient colorGradient)
    {
        if (particleDecalDataIndex >= maxDecals) // write over oldest decals once max is reached
        {
            particleDecalDataIndex = 0;
        }

        // record collision position, rotation, size and colour
        particleData[particleDecalDataIndex].position = particleCollisionEvent.intersection;
        // particleData[particleDecalDataIndex].position.x += Random.Range(0.00f, 0.100f); 
        particleData[particleDecalDataIndex].position.y += Random.Range(0.00f, 0.100f); 
        particleData[particleDecalDataIndex].position.z += Random.Range(0.00f, 0.100f); 


        if (particleCollisionEvent.normal.x > Mathf.Epsilon || particleCollisionEvent.normal.y > Mathf.Epsilon || particleCollisionEvent.normal.z > Mathf.Epsilon)
        {
            Vector3 particleRotationEuler = Quaternion.LookRotation(particleCollisionEvent.normal).eulerAngles;
            particleRotationEuler.z = Random.Range(0, 360);
            
            particleData[particleDecalDataIndex].rotation = particleRotationEuler;
        }

        particleData[particleDecalDataIndex].size = Random.Range(decalSizeMin, decalSizeMax);

        particleData[particleDecalDataIndex].color = colorGradient.Evaluate(Random.Range(0f, 1f));


        particleDecalDataIndex++; // next index to save details for next splatter particle
    }

    // void DisplayParticles()
    // {
    //     // set each splatter decals' data
    //     for (int i = 0; i < particleData.Length; i++)
    //     {
    //         particles[i].position = particleData[i].position;
    //         particles[i].rotation3D = particleData[i].rotation;
    //         particles[i].startSize = particleData[i].size;
    //         particles[i].startColor = particleData[i].color;
    //     }

    //     decalParticleSystem.SetParticles(particles, particles.Length); // SetParticles is a class of a particle system
    // }
}
