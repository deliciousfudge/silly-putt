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

    private Rigidbody BallRigidBody;
    private S_GameCamera CameraScript;
    private Vector3 v3Direction = new Vector3(0.0f, 0.0f, 0.0f);
    private float fMaxSpeed = 200.0f;
    private float fCurrentSpeed = 0.0f;

    private bool bIsHoldingLeftMouseButtonDown = false;
    private Vector3 v3MouseLocation = new Vector3(0.0f, 0.0f, 0.0f);

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
        if (m_bHasRoundStarted && !m_bHasRoundEnded)
        {
            if (m_bIsBallMoving)
            {
                // If the velocity of the ball has reduced to a significantly slow amount
                if (BallRigidBody.velocity.sqrMagnitude < 0.7f)
                {
                    // Remove all velocity from the ball
                    RemoveVelocity();
                }
            }
            else
            {
                // If the player presses the left mouse button
                if (Input.GetMouseButtonDown(0))
                {
                    // Mark the button as being held down
                    bIsHoldingLeftMouseButtonDown = true;
                }

                // If the left mouse button is still held down
                if (bIsHoldingLeftMouseButtonDown)
                {
                    // Update the stored location of the mouse pointer in world space
                    v3MouseLocation = cursorOnTransform;

                    v3MouseLocation.y = 0.0f;

                    Debug.DrawLine(BallRigidBody.position, v3MouseLocation);
                }

                // If the left mouse button is released
                if (Input.GetMouseButtonUp(0))
                {
                    bIsHoldingLeftMouseButtonDown = false;

                    // If a valid mouse pointer location is stored
                    if (v3MouseLocation != new Vector3(0.0f, 0.0f, 0.0f))
                    {
                        // Execute the shot
                        Vector3 SwingDirection = transform.position - v3MouseLocation;

                        fCurrentSpeed = Mathf.Clamp(SwingDirection.magnitude, fMaxSpeed * 0.1f, fMaxSpeed);

                        v3Direction = SwingDirection.normalized;

                        v3Direction.y = 0.0f;

                        BallRigidBody.AddForceAtPosition(v3Direction * fCurrentSpeed, BallRigidBody.position, ForceMode.Impulse);

                        // Mark the ball as moving
                        m_bIsBallMoving = true;

                        // Unlock the small steps achievement (for hitting the ball)
                        PlayerPrefs.SetInt("Small Steps", 0);

                        // Increment the shot count
                        m_iShotCount++;
                    }

                    v3MouseLocation = new Vector3(0.0f, 0.0f, 0.0f);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Hole")
        {
            RemoveVelocity();

            BallRigidBody.detectCollisions = false;

            CameraScript.m_bIsFollowingBall = false;

            m_bHasRoundEnded = true;
        }
    }

    private static Vector3 cursorWorldPosOnNCP
    {
        get
        {
            return Camera.main.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x,
                Input.mousePosition.y,
                Camera.main.nearClipPlane));
        }
    }

    private static Vector3 cameraToCursor
    {
        get
        {
            return cursorWorldPosOnNCP - Camera.main.transform.position;
        }
    }

    private Vector3 cursorOnTransform
    {
        get
        {
            Vector3 camToTrans = transform.position - Camera.main.transform.position;
            return Camera.main.transform.position +
                cameraToCursor *
                (Vector3.Dot(Camera.main.transform.forward, camToTrans) / Vector3.Dot(Camera.main.transform.forward, cameraToCursor));
        }
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.green;
        //Gizmos.DrawLine(transform.position, v3MouseLocation);
        //print("Ball pos");
        //print(transform.position);
        //print("Mouse pos");
        //print(v3MouseLocation);
    }

    public void ResetState()
    {
        RemoveVelocity();
        SetCollision(true);
        ResetPosition();
    }

    private void RemoveVelocity()
    {
        BallRigidBody.velocity = Vector3.zero;
        BallRigidBody.angularVelocity = Vector3.zero;
        m_bIsBallMoving = false;
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
