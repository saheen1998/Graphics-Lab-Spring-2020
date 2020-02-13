using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CamConstraint_Orbit : MonoBehaviour
{
    public float rotateSpeed = 8f;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            float h = rotateSpeed * Input.GetAxis("Mouse X");
            float v = rotateSpeed * Input.GetAxis("Mouse Y");

            if (transform.eulerAngles.z + v <= 0.1f || transform.eulerAngles.z + v >= 179.9f)
                v = 0;

            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + h, transform.eulerAngles.z + v);
        }

        float scrollFactor = Input.GetAxis("Mouse ScrollWheel");

        if (scrollFactor != 0)
        {
            transform.localScale = transform.localScale * (1f - scrollFactor);
        }

    }
}
