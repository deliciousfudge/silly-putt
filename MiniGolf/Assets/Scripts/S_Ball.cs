using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class S_Ball : MonoBehaviour
{
    public UnityEvent OnBallHasEnteredHole;

    public Vector3 m_v3StartingPos = new Vector3(-1.5f, 0.45f, -8.0f);
    public bool m_bHasRoundStarted = false;
    public bool m_bHasRoundEnded = false;
    public bool m_bIsBallMoving = false;
    public bool m_bHasBallEnteredHole = false;
    public bool m_bHasHitBomb = false;
    public int m_iShotCount = 0;
    public float m_fMaxSpeed = 20.0f;

    private Rigidbody m_RigidBodyBall;
    private Vector3 m_v3Direction = new Vector3(0.0f, 0.0f, 0.0f);
    private float m_fCurrentSpeed = 0.0f;
    private float m_fProjectedDistance = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        // Set script references
        m_RigidBodyBall = GetComponent<Rigidbody>();
    }

    public void PerformShot(Vector3 _Direction, float _PowerRatio)
    {
        m_v3Direction = _Direction;
        m_v3Direction.y = 0.0f;

        m_fCurrentSpeed = m_fMaxSpeed * _PowerRatio;

        m_RigidBodyBall.AddForceAtPosition(m_v3Direction * m_fCurrentSpeed, m_RigidBodyBall.position, ForceMode.Impulse);

        // Mark the ball as moving
        m_bIsBallMoving = true;

        // Increment the shot count
        m_iShotCount++;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Hole")
        {
            RemoveVelocity();

            m_RigidBodyBall.detectCollisions = false;

            OnBallHasEnteredHole.Invoke();

            StartCoroutine(MarkEndOfRound());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bomb")
        {
            collision.gameObject.SetActive(false);

            m_bHasHitBomb = true;
        }
    }

    public void ResetState(bool _bShouldResetShotCount = true)
    {
        RemoveVelocity();

        SetCollision(true);

        ResetPosition();

        if (_bShouldResetShotCount)
        {
            m_iShotCount = 0;
        }

        m_bHasRoundEnded = false;

        m_bHasRoundStarted = false;
    }

    public void SetProjectedDistance(float _ProjectedDistance)
    {
        m_fProjectedDistance = _ProjectedDistance;
    }

    public void RemoveVelocity()
    {
        m_RigidBodyBall.velocity = Vector3.zero;
        m_RigidBodyBall.angularVelocity = Vector3.zero;

        m_bIsBallMoving = false;
    }

    public Rigidbody GetBallRigidbody()
    {
        return m_RigidBodyBall;
    }

    private IEnumerator MarkEndOfRound()
    {
        yield return new WaitForSeconds(0.3f);
        m_bHasRoundEnded = true;
    }

    private void SetCollision(bool _bIsColliding)
    {
        if (_bIsColliding)
        {
            m_RigidBodyBall.detectCollisions = true;
        }
        else
        {
            m_RigidBodyBall.detectCollisions = false;
        }
    }

    private void ResetPosition()
    {
        m_RigidBodyBall.transform.position = m_v3StartingPos;
    }
}
