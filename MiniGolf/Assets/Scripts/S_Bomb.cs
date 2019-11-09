using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Bomb : MonoBehaviour
{
    public float m_fMovementSpeed = 0.1f;
    public bool m_bCanMove = true;

    private Vector3 m_v3MovementDir;
    private bool m_bHasBeenBlownUp = false;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall")
        {
            m_v3MovementDir *= -1.0f;
        }
    }

    public bool GetHasBeenBlownUp()
    {
        return m_bHasBeenBlownUp;
    }

    public void SetHasBeenBlownUp(bool _bHasBeenBlownUp)
    {
        m_bHasBeenBlownUp = _bHasBeenBlownUp;
    }
}
