using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using System.Runtime.InteropServices;

public class FileUploader : MonoBehaviour
{
    [DllImport("__Internal")] private static extern void AddClickListenerForFileDialog();
    [DllImport("__Internal")] private static extern void FocusFileUploader();

    void Start()
    {
        AddClickListenerForFileDialog();
    }

    void OnPointerDown(){
        Debug.Log("Clicked!");
        //FocusFileUploader();
    }

    public void Click(){
        Debug.Log("Clicked!");
        FocusFileUploader();
    }

    public void FileDialogResult(string fileUrl)
    {
        Debug.Log(fileUrl);
        //UrlTextField.text = fileUrl;
        StartCoroutine(LoadBlob(fileUrl));
    }

    IEnumerator LoadBlob(string url)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();

        if (!webRequest.isNetworkError && !webRequest.isHttpError)
        {
            // Get text content like this:
            Debug.Log(webRequest.downloadHandler.text);

        }
    }
}
