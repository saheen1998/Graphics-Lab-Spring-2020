using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptCollision : MonoBehaviour
{

    public GameObject collisionAlert;
    private void OnTriggerStay(Collider other) {
        collisionAlert.SetActive(true);
    }

    private void OnTriggerExit(Collider other) {
        collisionAlert.SetActive(false);
    }
}
