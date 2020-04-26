using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script for checking robot arm collision with table
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
