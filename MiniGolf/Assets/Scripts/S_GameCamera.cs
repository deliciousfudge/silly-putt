﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_GameCamera : MonoBehaviour
{
    public GameObject m_Ball;

    private Camera m_Camera;
    public Vector3 m_v3CameraOffset = new Vector3(0.0f, 0.0f, -5.0f);
    public float m_fCameraHeight = 2.0f;

    public bool bIsFollowingBall = true;

    // Start is called before the first frame update
    void Start()
    {
        m_Camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bIsFollowingBall)
        {
            m_Camera.transform.position = Vector3.Lerp(m_Ball.transform.position, m_Ball.transform.position + m_v3CameraOffset, 2.0f);
            m_Camera.transform.position = new Vector3(m_Camera.transform.position.x, m_fCameraHeight, m_Camera.transform.position.z);
        }
    }
}
