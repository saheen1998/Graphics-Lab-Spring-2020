using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardKinematicsUR5 : MonoBehaviour
{
    public List<Transform> arm;

    public void ModifyPos(List<float> ang) {
        arm[0].localRotation = Quaternion.Euler(0, ang[0] * Mathf.Rad2Deg, 0);
        arm[1].localRotation = Quaternion.Euler(ang[1] * Mathf.Rad2Deg, 0, 0);
        arm[3].localRotation = Quaternion.Euler(-ang[2] * Mathf.Rad2Deg, 0, 0);
        arm[5].localRotation = Quaternion.Euler(ang[3] * Mathf.Rad2Deg, 0, 0);
        arm[6].localRotation = Quaternion.Euler(0, ang[4] * Mathf.Rad2Deg, 0);
    }

    public Vector3 GetPoint(){
        return arm[7].transform.position;
    }

    public Quaternion GetRotation() {
        return arm[6].transform.rotation;
    }
}
