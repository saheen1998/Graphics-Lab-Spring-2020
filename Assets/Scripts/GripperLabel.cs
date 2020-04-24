using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GripperLabel : MonoBehaviour
{
    public GameObject cameraToLookAt;
 
    // Use this for initialization 
    void Start()
    {
        cameraToLookAt = GameObject.Find("CamFree");
    }

    // Update is called once per frame 
    void LateUpdate()
    {
        transform.LookAt(cameraToLookAt.transform);
        transform.rotation = Quaternion.LookRotation(cameraToLookAt.transform.forward);
    }
}
