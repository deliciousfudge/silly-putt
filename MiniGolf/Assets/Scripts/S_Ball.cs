using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Ball : MonoBehaviour
{
    public Camera Camera;

    private Rigidbody RB;
    private bool bCanBallBeHit = true;
    private S_GameCamera CameraScript;
    private Vector3 v3Direction = new Vector3(0.0f, 0.0f, 0.0f);
    private float fMaxSpeed = 200.0f;
    private float fCurrentSpeed = 0.0f;
    private float fFrictionFactor = 0.05f;

    private bool bIsHoldingLeftMouseButtonDown = false;
    private Vector3 v3MouseLocation = new Vector3(0.0f, 0.0f, 0.0f);

    // Start is called before the first frame update
    void Start()
    {
        print("Get in the hole!");

        RB = GetComponent<Rigidbody>();
        CameraScript = Camera.GetComponent<S_GameCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bCanBallBeHit)
        {
            if (Input.GetMouseButtonDown(0))
            {
                print("Left button clicked");

                bIsHoldingLeftMouseButtonDown = true;
            }

            if (bIsHoldingLeftMouseButtonDown)
            {
                v3MouseLocation = cursorOnTransform;

                v3MouseLocation.y = 0.0f;

                Debug.DrawLine(RB.position, v3MouseLocation);
                print("Ball pos");
                print(transform.position);
                print("Mouse pos");
                print(v3MouseLocation);
            }

            if (Input.GetMouseButtonUp(0))
            {
                bIsHoldingLeftMouseButtonDown = false;

                if (v3MouseLocation != new Vector3(0.0f, 0.0f, 0.0f))
                {
                    Vector3 SwingDirection = transform.position - v3MouseLocation;

                    fCurrentSpeed = Mathf.Clamp(SwingDirection.magnitude, fMaxSpeed * 0.1f, fMaxSpeed);

                    v3Direction = SwingDirection.normalized;

                    v3Direction.y = 0.0f;

                    RB.AddForceAtPosition(v3Direction * fCurrentSpeed, RB.position, ForceMode.Impulse);

                    bCanBallBeHit = false;
                }

                v3MouseLocation = new Vector3(0.0f, 0.0f, 0.0f);
            }
        }
        else
        {
            //print(RB.velocity.sqrMagnitude);

            if (RB.velocity.sqrMagnitude < 0.7f)
            {
                RB.velocity = Vector3.zero;
                RB.angularVelocity = Vector3.zero;
                bCanBallBeHit = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Hole")
        {
            print("Dancin'!");

            bCanBallBeHit = false;

            RB.velocity = Vector3.zero;
            RB.angularVelocity = Vector3.zero;
            RB.detectCollisions = false;

            CameraScript.bIsFollowingBall = false;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Wall")
        {

            print("Ah my face!");
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
}
