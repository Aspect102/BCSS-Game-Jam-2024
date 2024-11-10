using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public GameObject player;
    public int damage;
    public float speed;

    public LayerMask groundMask;
    public float groundDistance;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");

        Transform playerTransform = player.transform;
        transform.forward = (playerTransform.position - transform.position).normalized;

        Destroy(gameObject, 5f);
    }


    // Update is called once per frame
    void Update()
    {
        if(Physics.CheckSphere(transform.position, groundDistance, groundMask))
        {
            PlayerCombat playerCombat = player.GetComponent<PlayerCombat>();
            playerCombat.TakeDamage(damage);
            Destroy(gameObject);
        }

        transform.Translate(Vector3.forward * Time.deltaTime * speed);

    }
}
