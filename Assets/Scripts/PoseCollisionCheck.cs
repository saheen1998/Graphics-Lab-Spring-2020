using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseCollisionCheck : MonoBehaviour
{
    bool finalized = false;

    void Start() {
        StartCoroutine(FinalizePosition(0.1f));
    }

    IEnumerator FinalizePosition(float t) {
        yield return new WaitForSeconds(t);
        finalized = true;
    }
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("DupGrip") && finalized == false) {
            RobotControllerScript.posePts.RemoveAt(RobotControllerScript.posePts.Count - 1);
            RobotControllerScript.poseTimes.RemoveAt(RobotControllerScript.poseTimes.Count - 1);
            RobotControllerScript.dupGrippers.RemoveAt(RobotControllerScript.dupGrippers.Count - 1);
            Destroy(gameObject);
        }
    }
}
