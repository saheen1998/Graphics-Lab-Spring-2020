using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardKinematics : MonoBehaviour
{
    public List<Transform> arm;

    public Vector3 GetPoint(List<float> ang){

        arm[0].localRotation = Quaternion.Euler(0, ang[0] * Mathf.Rad2Deg, 0);
        arm[1].localRotation = Quaternion.Euler(0, 0, ang[1] * Mathf.Rad2Deg);
        arm[2].localRotation = Quaternion.Euler(-ang[2] * Mathf.Rad2Deg, 0, 0);
        arm[3].localRotation = Quaternion.Euler(0, 0, ang[3] * Mathf.Rad2Deg);
        arm[4].localRotation = Quaternion.Euler(-ang[4] * Mathf.Rad2Deg, 0, 0);
        arm[5].localRotation = Quaternion.Euler(0, 0, ang[5] * Mathf.Rad2Deg);
        arm[6].localRotation = Quaternion.Euler(ang[6] * Mathf.Rad2Deg, 0, 0);

        return arm[7].transform.position;
    }
}
