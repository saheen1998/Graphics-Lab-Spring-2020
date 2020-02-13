using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GripperScript : MonoBehaviour
{
    public Transform rightGrip;
    public Transform leftGrip;

    private HingeJoint right;
    private HingeJoint left;

    private bool close = false;
    // Start is called before the first frame update
    void Start()
    {
        right = rightGrip.GetChild(0).gameObject.GetComponent<HingeJoint>();
        left = leftGrip.GetChild(0).gameObject.GetComponent<HingeJoint>();
    }

    // Update is called once per frame

    public void Open(){
        if(!close)
            return;

        close = false;

        var rightMotor = right.motor;
        var leftMotor = left.motor;

        rightMotor.targetVelocity = -150;
        leftMotor.targetVelocity = -150;

        right.motor = rightMotor;
        left.motor = leftMotor;
    }

    public void Close(){
        if(close)
            return;

        close = true;

        var rightMotor = right.motor;
        var leftMotor = left.motor;

        rightMotor.targetVelocity = 150;
        leftMotor.targetVelocity = 150;

        right.motor = rightMotor;
        left.motor = leftMotor;
    }
}
