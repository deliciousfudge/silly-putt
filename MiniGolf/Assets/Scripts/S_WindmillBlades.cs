﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_WindmillBlades : MonoBehaviour
{
    public float m_fRotationRate = 20.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0.0f, 0.0f, Time.deltaTime * m_fRotationRate));
    }
}
