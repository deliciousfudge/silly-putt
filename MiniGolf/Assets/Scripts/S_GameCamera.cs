using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_GameCamera : MonoBehaviour
{
    [Header("RotateButtons")]
    public Button InGameRotateCameraClockwise;
    public Button InGameRotateCameraCounterClockwise;

    [Header("Camera Configuration")]
    public Vector3 m_v3CameraOffset = new Vector3(0.0f, 0.0f, -5.0f);
    public bool m_bShouldFollowBall = true;
    public float m_fCameraYRotationRate = 2.0f;
    public float m_fCameraPitch = 35.0f;

    private Camera m_Camera;
    private float m_fCameraYRotationDelta { get; set; } = 0.0f;
    private bool m_bIsRotateClockwiseHeld { get; set; } = false;
    private bool m_bIsRotateCounterClockwiseHeld { get; set; } = false;
    private bool m_bIsPlayerSelectingShot = true;
    
    private Transform m_v3BallTransform;
    private Vector3 m_v3BallDirection;

    private Vector3 m_v3CurrentPosition;
    private Vector3 m_v3NewPosition;

    // Start is called before the first frame update
    void Start()
    {
        m_Camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_bShouldFollowBall)
        {
            m_v3CurrentPosition = m_Camera.transform.position;

            ProcessCameraPositioning();
        }
    }

    public void ResetState()
    {
        m_bShouldFollowBall = true;

        m_Camera.transform.rotation = Quaternion.Euler(new Vector3(35.0f, 0.0f, 0.0f));

        SetIsPlayerSelectingShot(true);
    }

    public void OnRotateClockwiseButtonPressed()
    {
        m_bIsRotateClockwiseHeld = true;
    }

    public void OnRotateClockwiseButtonReleased()
    {
        m_bIsRotateClockwiseHeld = false;
    }

    public void OnRotateCounterClockwiseButtonPressed()
    {
        m_bIsRotateCounterClockwiseHeld = true;
    }

    public void OnRotateCounterClockwiseButtonReleased()
    {
        m_bIsRotateCounterClockwiseHeld = false;
    }

    public void SetBallTransform(Transform _NewTransform)
    {
        m_v3BallTransform = _NewTransform;
    }

    public void SetBallDirection(Vector3 _NewDirection)
    {
        m_v3BallDirection = _NewDirection;
    }

    public void SetIsPlayerSelectingShot(bool _IsSelectingShot)
    {
        m_bIsPlayerSelectingShot = _IsSelectingShot;

        if (_IsSelectingShot)
        {
            EnableRotation();
        }
        else
        {
            DisableRotation();
        }
    }

    public void PursueBall ()
    {
        m_Camera.transform.position = Vector3.Lerp(m_Camera.transform.position, m_v3BallTransform.position + (m_v3BallDirection * -3.0f), Time.deltaTime * 10.0f);
    }

    public void ProcessCameraPositioning()
    {
        m_v3NewPosition = m_v3BallTransform.position;

        m_Camera.transform.SetParent(m_v3BallTransform);

        if (m_bIsRotateClockwiseHeld)
        {
            m_Camera.transform.Rotate(Vector3.up, Time.deltaTime * m_fCameraYRotationRate);
        }
        else if (m_bIsRotateCounterClockwiseHeld)
        {
            m_Camera.transform.Rotate(Vector3.up, Time.deltaTime * -m_fCameraYRotationRate);
        }

        m_v3NewPosition += m_Camera.transform.forward * m_v3CameraOffset.z;
        m_v3NewPosition = new Vector3(m_v3NewPosition.x, m_v3CameraOffset.y, m_v3NewPosition.z);

        m_Camera.transform.rotation = Quaternion.Euler(m_fCameraPitch, m_Camera.transform.eulerAngles.y, 0.0f);

        m_Camera.transform.SetParent(null);

        m_Camera.transform.position = Vector3.Lerp(m_v3CurrentPosition, m_v3NewPosition, Time.deltaTime * 5.0f);
    }

    public void DisableRotation()
    {
        InGameRotateCameraClockwise.gameObject.SetActive(false);
        InGameRotateCameraCounterClockwise.gameObject.SetActive(false);
    }

    public void EnableRotation()
    {
        InGameRotateCameraClockwise.gameObject.SetActive(true);
        InGameRotateCameraCounterClockwise.gameObject.SetActive(true);
    }

    public Camera GetCamera()
    {
        return m_Camera;
    }
}