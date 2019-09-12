using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_WindmillBlades : MonoBehaviour
{
    public GameObject BladePivotPoint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0.0f, 0.0f, Time.deltaTime * 10.0f));
        transform.position = BladePivotPoint.transform.position + new Vector3(1.9f, 2.7f, -2.9f);
    }
}
