using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class LogHandler : MonoBehaviour
{
    private string logFilePath;
    private StreamWriter writer;
    private DialogModule msgModule;

    public static LogHandler Logger{ get; private set; }

    void Awake(){
        Logger = this;
    }

    void Start()
    {
        msgModule = gameObject.GetComponent<DialogModule>();
        logFilePath = Application.dataPath + "/log.txt";
        writer = new StreamWriter(logFilePath);
    }

    public void Log(string logString, LogType type){
        switch (type)
        {
            case LogType.Warning: Debug.LogWarning(logString);
                break;
            case LogType.Error: Debug.LogError(logString);
                break;
            default: Debug.Log(logString);
                break;
        }
        writer.WriteLine(System.DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + " - " + type + "\n\t" + logString);
        writer.Flush();
    }

    public void ShowMessage(string msg, string title){
        msgModule.ShowMessage(msg);
    }
}
