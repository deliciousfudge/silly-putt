using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_ProjectedPath : MonoBehaviour
{
    public GameObject BallObject;

    private Image ArrowImage;

    // Start is called before the first frame update
    void Start()
    {
        ArrowImage = GetComponentInChildren<Image>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
