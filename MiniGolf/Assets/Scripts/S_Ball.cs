using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Ball : MonoBehaviour
{
    public Camera Camera;
    public GameObject GameManager;
    public Vector3 m_v3StartingPos = new Vector3(-1.5f, 0.45f, -8.0f);
    public bool m_bHasRoundStarted = false;
    public bool m_bHasRoundEnded = false;
    public bool m_bIsBallMoving = false;
    public int m_iShotCount = 0;
    public float m_fMaxSpeed = 20.0f;

    private Rigidbody BallRigidBody;
    private LineRenderer BallLineRenderer;
    private S_GameCamera CameraScript;
    private Vector3 m_v3Direction = new Vector3(0.0f, 0.0f, 0.0f);
    private float m_fCurrentSpeed = 0.0f;
    private float m_fProjectedDistance = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        // Set script references
        BallRigidBody = GetComponent<Rigidbody>();
        CameraScript = Camera.GetComponent<S_GameCamera>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PerformShot(Vector3 _Direction, float _PowerRatio)
    {
        m_v3Direction = _Direction;
        m_v3Direction.y = 0.0f;

        m_fCurrentSpeed = m_fMaxSpeed * _PowerRatio;

        BallRigidBody.AddForceAtPosition(m_v3Direction * m_fCurrentSpeed, BallRigidBody.position, ForceMode.Impulse);

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

            BallRigidBody.detectCollisions = false;

            CameraScript.m_bShouldFollowBall = false;

            StartCoroutine(MarkEndOfRound());

        }
    }

    private void OnDrawGizmos()
    {
        if (CameraScript != null && !m_bIsBallMoving)
        {
            //Gizmos.DrawLine(transform.position, transform.position + (CameraScript.transform.forward * m_fProjectedDistance));
            //Gizmos.DrawIcon(transform.position + (CameraScript.transform.forward), "arrowUp", false);

            if (m_fProjectedDistance > 0.02f)
            {
                for (int i = 1; i < 6; ++i)
                {
                    print((m_fProjectedDistance / i));
                    Vector3 ForwardVec = CameraScript.transform.forward * (m_fProjectedDistance / i);
                    Vector3 ProjectedPos = transform.position + ForwardVec;
                    ProjectedPos.y = transform.position.y;

                    Gizmos.DrawSphere(ProjectedPos, 0.05f);
                }
            }

        }
    }

    public void ResetState()
    {
        RemoveVelocity();
        SetCollision(true);
        ResetPosition();

        m_iShotCount = 0;
        m_bHasRoundEnded = false;
        m_bHasRoundStarted = false;
    }

    public void SetProjectedDistance(float _ProjectedDistance)
    {
        m_fProjectedDistance = _ProjectedDistance;
    }

    public void RemoveVelocity()
    {
        BallRigidBody.velocity = Vector3.zero;
        BallRigidBody.angularVelocity = Vector3.zero;
        m_bIsBallMoving = false;
    }

    public Rigidbody GetBallRigidbody()
    {
        return BallRigidBody;
    }

    private IEnumerator MarkEndOfRound()
    {
        yield return new WaitForSeconds(0.5f);
        m_bHasRoundEnded = true;
    }

    private void SetCollision(bool _bIsColliding)
    {
        if (_bIsColliding)
        {
            BallRigidBody.detectCollisions = true;
        }
        else
        {
            BallRigidBody.detectCollisions = false;
        }
    }

    private void ResetPosition()
    {
        BallRigidBody.transform.position = m_v3StartingPos;
    }
}
