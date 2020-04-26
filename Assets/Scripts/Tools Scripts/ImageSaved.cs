using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script for the 'Image Saved' alert box
public class ImageSaved : MonoBehaviour
{
    void OnEnable()
    {
        StartCoroutine(deactivate());
    }

    public void activate() {
        gameObject.SetActive(true);
    }

    IEnumerator deactivate() {
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
    }
}
