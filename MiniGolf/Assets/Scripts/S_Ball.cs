using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Ball : MonoBehaviour
{
    public Camera Camera;

    private bool bCanBallBeMoved = true;
    private S_GameCamera CameraScript;
    private Vector3 v3Direction = new Vector3(0.0f, 0.0f, 0.0f);
    private float fMaxSpeed = 0.3f;
    private float fCurrentSpeed = 0.0f;
    private float fFrictionFactor = 0.05f;

    private bool bIsHoldingLeftMouseButtonDown = false;
    private Vector3 v3MouseLocation = new Vector3(0.0f, 0.0f, 0.0f);

    // Start is called before the first frame update
    void Start()
    {
        print("Get in the hole!");

        CameraScript = Camera.GetComponent<S_GameCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bCanBallBeMoved)
        {
            if (fCurrentSpeed < 0.01f)
            {
                fCurrentSpeed = 0.0f;
            }
            else
            {
                // Apply friction to the ball
                fCurrentSpeed *= (1.0f - fFrictionFactor);

                transform.position += v3Direction * fCurrentSpeed; 
            }

            if (fCurrentSpeed == 0.0f)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    print("Left button clicked");

                    bIsHoldingLeftMouseButtonDown = true;
                }
            }

            if (bIsHoldingLeftMouseButtonDown)
            {
                v3MouseLocation = cursorOnTransform;

                v3MouseLocation.y = 0.0f;

                //print(v3MouseLocation);
            }

            if (Input.GetMouseButtonUp(0))
            {
                bIsHoldingLeftMouseButtonDown = false;

                if (v3MouseLocation != new Vector3(0.0f, 0.0f, 0.0f))
                {
                    Vector3 SwingDirection = transform.position - v3MouseLocation;

                    fCurrentSpeed = Mathf.Clamp(SwingDirection.magnitude, 0.0f, fMaxSpeed);

                    v3Direction = SwingDirection.normalized;

                    v3Direction.y = 0.0f;

                    print(v3Direction);

                    print(fCurrentSpeed);
                }

                v3MouseLocation = new Vector3(0.0f, 0.0f, 0.0f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Hole")
        {
            print("Dancin'!");

            bCanBallBeMoved = false;

            CameraScript.bIsFollowingBall = false;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Wall")
        {
            Vector3 NewDirection = Vector3.Reflect(v3Direction, other.contacts[0].normal);

            v3Direction.x = NewDirection.x;
            v3Direction.z = NewDirection.z;

            fCurrentSpeed = fCurrentSpeed * 0.9f;
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
}
