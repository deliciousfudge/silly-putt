using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float m_fMovementSpeed = 0.1f;
    public bool m_bCanMove = true;

    private Vector3 m_v3MovementDir;

    // Start is called before the first frame update
    void Start()
    {
        if (m_bCanMove)
        {
            int RandomDir = Random.Range(0, 1);

            if (RandomDir == 0)
            {
                m_v3MovementDir = gameObject.transform.right * -1.0f;
            }
            else
            {
                m_v3MovementDir = gameObject.transform.right;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position += m_v3MovementDir * m_fMovementSpeed;
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag == "Wall")
    //    {
    //        print("Ah my face");
    //        m_v3MovementDir *= -1.0f;
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall")
        {
            print("Ah my face");
            m_v3MovementDir *= -1.0f;
        }
    }
}
