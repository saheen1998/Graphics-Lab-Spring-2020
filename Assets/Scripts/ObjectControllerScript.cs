using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectControllerScript : MonoBehaviour
{
    public Transform table;
    public Slider heightSlider;
    public Slider distSlider;

    public GameObject collisionAlert;

    public void ToggleTable(bool stat){
        table.gameObject.SetActive(stat);
        collisionAlert.SetActive(false);
    }

    public void SetTableHeight(float h) {
        table.position = new Vector3(table.position.x, h - 0.04f, table.position.z);
        heightSlider.gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = (-(-table.position.y - 0.04f - 0.9249f)).ToString("F4");
    }
    
    public void SetTableDist(float d) {
        table.position = new Vector3(d, table.position.y, table.position.z);
        distSlider.gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = (table.position.x).ToString("F4");
    }
}