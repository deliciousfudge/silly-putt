using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody RB;
    private Camera Camera;
    private bool MoveBall = true;

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
            transform.position += new Vector3(0.001f, 0.001f, 0.025f);
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Hole")
        {
            print("Dancin'!");

            MoveBall = false;
        }
    }
}
