using System.Collections;
using UnityEngine;

public class BallBounce : MonoBehaviour
{
    public float bounceForce = 10f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        rb.AddForce(collision.contacts[0].normal * bounceForce, ForceMode.Impulse);
    }
}