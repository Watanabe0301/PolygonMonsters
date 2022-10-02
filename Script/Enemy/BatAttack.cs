using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatAttack : MonoBehaviour
{
    private Rigidbody rb;
    protected int ATK = 3;
    [SerializeField] private float speed = 1.5f;
    void Start()
    {
        //batStatus = transform.parent.GetComponent<Enemy>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, 5f);
    }

    void OnTriggerEnter(Collider other)
    {
        Vector3 hitPos = other.ClosestPointOnBounds(this.transform.position);
        //UŒ‚‚µ‚½‘Šè‚ªEnemy‚Ìê‡
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerMotion>().Damage(ATK);
        }


    }
}
