using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Forward Kinematics implemented to get the trajectory of the robot from the joint angle values
    * These functions are to be called in a loop to continuously modify joint states
    * and return position and rotation at that instant
*/
public class ForwardKinematics : MonoBehaviour
{
    public List<Transform> arm;

    // Modify the joint angles using the argument 'ang'
    public void ModifyPos(List<float> ang) {
        arm[0].localRotation = Quaternion.Euler(0, ang[0] * Mathf.Rad2Deg, 0);
        arm[1].localRotation = Quaternion.Euler(0, 0, ang[1] * Mathf.Rad2Deg);
        arm[2].localRotation = Quaternion.Euler(-ang[2] * Mathf.Rad2Deg, 0, 0);
        arm[3].localRotation = Quaternion.Euler(0, 0, ang[3] * Mathf.Rad2Deg);
        arm[4].localRotation = Quaternion.Euler(-ang[4] * Mathf.Rad2Deg, 0, 0);
        arm[5].localRotation = Quaternion.Euler(0, 0, ang[5] * Mathf.Rad2Deg);
        arm[6].localRotation = Quaternion.Euler(-ang[6] * Mathf.Rad2Deg, 0, 0);
    }

    // Get the current end-effector point
    public Vector3 GetPoint(){
        return arm[7].transform.position;
    }

    // Get the current rotation of the end-effector
    public Quaternion GetRotation() {
        return arm[6].transform.rotation;
    }
}
