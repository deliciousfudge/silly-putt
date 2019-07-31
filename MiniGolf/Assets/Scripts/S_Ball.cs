using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Ball : MonoBehaviour
{
    private Rigidbody RB;
    private Camera Camera;
    private bool MoveBall = true;

    private Vector3 v3Direction = new Vector3(0.5f, 0.0f, 1.0f);
    private float fSpeed = 0.1f;
    private float fFrictionFactor = 0.001f;

    // Start is called before the first frame update
    void Start()
    {
        print("Get in the hole!");
    }

    // Update is called once per frame
    void Update()
    {
        if (MoveBall)
        {
            // Apply friction to the ball
            fSpeed = fSpeed * (1.0f - fFrictionFactor);

            transform.position += v3Direction * fSpeed;
            transform.Rotate(0.1f, 0.0f, 0.0f);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Hole")
        {
            print("Dancin'!");

            MoveBall = false;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Hole")
        {
            print("Dancin'!");

            MoveBall = false;
        }
        else if (other.gameObject.tag == "Wall")
        {
            v3Direction = Vector3.Reflect(v3Direction, other.contacts[0].normal);
            fSpeed = fSpeed * 0.9f;
            print("Ah my face!");
        }
    }
}
