using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerKnuckleScript : MonoBehaviour
{
    public Transform joint;
    // Update is called once per frame
    void Update()
    {
        transform.rotation = joint.rotation;
    }
}
