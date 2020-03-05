using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GraphScript : MonoBehaviour
{
    public Text textForce;
    public Sprite ptSprite;
    public Sprite lineSprite;
    public RectTransform graphContainer;

    private float height;
    private float width;
    private float ymax = 6.28f;
    private RectTransform currLineRect;

    private List<double> forces;
    private int count;

    private void Awake() {
        height = graphContainer.sizeDelta.y;
        width = graphContainer.sizeDelta.x;

        //Plot the current state time line
        GameObject line = new GameObject("Current State line", typeof(Image));
        line.transform.SetParent(graphContainer, false);
        line.GetComponent<Image>().sprite = lineSprite;
        currLineRect = line.GetComponent<RectTransform>();
        currLineRect.anchoredPosition = new Vector2(0, 0);
        currLineRect.sizeDelta = new Vector2(1, height);
        currLineRect.anchorMin = new Vector2(0, 0.5f);
        currLineRect.anchorMax = new Vector2(0, 0.5f);
    }

    void PlotPoint(Vector2 pos){
        GameObject pt = new GameObject("GraphPt", typeof(Image));
        pt.transform.SetParent(graphContainer, false);
        pt.gameObject.tag = "Graph Point";
        pt.GetComponent<Image>().sprite = ptSprite;
        RectTransform ptRect= pt.GetComponent<RectTransform>();
        ptRect.anchoredPosition = pos;
        ptRect.sizeDelta = new Vector2(1, 1);
        ptRect.anchorMin = new Vector2(0, 0);
        ptRect.anchorMax = new Vector2(0, 0);
    }

    public void ShowGraph(List<double> val){
        forces = new List<double>(val);
        count = val.Count;
        ymax = 1.1f * Math.Max( Math.Abs((float)val.Max()), Math.Abs((float)val.Min()) );
        for (int i = 0; i < count; i++)
        {
            float xPos =  ((float)(i) / (count-1)) * width;
            float yPos = (float)val[i] / ymax * height;
            PlotPoint(new Vector2(xPos, yPos));
        }
    }

    public void PlotLine(int i) {
        
        GameObject line = new GameObject("State line", typeof(Image));
        line.transform.SetParent(graphContainer, false);
        line.GetComponent<Image>().sprite = lineSprite;
        RectTransform lineRect = line.GetComponent<RectTransform>();
        lineRect.anchoredPosition = new Vector2(0, 0);
        lineRect.sizeDelta = new Vector2(1, height);
        lineRect.anchorMin = new Vector2(0, 0.5f);
        lineRect.anchorMax = new Vector2(0, 0.5f);
        lineRect.anchoredPosition = new Vector2((float)(i/(float)count) * width, 0);
    }

    public void UpdateCurrentState(float val){
        currLineRect.anchoredPosition = new Vector2(val * width, 0);
    }

    public void ReadData(){
        
		////////////Get each row from csv data file
		GameObject robotController = GameObject.Find("Sawyer RobotController");

        //List<

		////////////Get each row from csv data file
		/*StreamReader forceP_data;
		try{
			forceP_data = new StreamReader(UIController.GetComponent<UI_Controller>().forcePlayFilePath);
		}catch{
           	LogHandler.Logger.Log(gameObject.name + " - ForceGraphScript.cs: Playback force data file does not exist or cannot be read!", LogType.Error);
			LogHandler.Logger.ShowMessage("Playback force data file does not exist or cannot be read!", "Error!");
			return;
		}

        //Read playback data from joint data file
		string data;
		data = forceP_data.ReadLine();
        int i = 0;
        do{
			string[] PData = data.Split(new char[] {' '} );

            forcesP.Add(double.Parse(PData[0]));

            i++;
			data = forceP_data.ReadLine();
		}while(data != null);*/
    }
}