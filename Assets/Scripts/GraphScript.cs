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
    public Sprite ptSprite;
    public Sprite lineSprite;
    public RectTransform graphContainer;

    private float height;
    private float width;
    private float ymax = 6.28f;
    private RectTransform currLineRect;

    private List<double> forces;
    private int count;
    private List<double[]> valuesToWrite1 = new List<double[]>();
    private List<double[]> valuesToWrite2 = new List<double[]>();
    private List<double[]> valuesToWrite3 = new List<double[]>();
    private List<double[]> valuesToWrite4 = new List<double[]>();

    private void Start() {
        height = graphContainer.rect.height;
        width = graphContainer.rect.width;

        //Plot the current state time line
        /*GameObject line = new GameObject("Current State line", typeof(Image));
        line.transform.SetParent(graphContainer, false);
        line.GetComponent<Image>().sprite = lineSprite;
        currLineRect = line.GetComponent<RectTransform>();
        currLineRect.anchoredPosition = new Vector2(0, 0);
        currLineRect.sizeDelta = new Vector2(1, height);
        currLineRect.anchorMin = new Vector2(0, 0.5f);
        currLineRect.anchorMax = new Vector2(0, 0.5f);*/
    }

    void PlotPoint(Vector2 pos, Color c){
        GameObject pt = new GameObject("GraphPt", typeof(Image));
        pt.transform.SetParent(graphContainer, false);
        pt.gameObject.tag = "Graph Point";
        pt.GetComponent<Image>().sprite = ptSprite;
        pt.GetComponent<Image>().color = c;
        RectTransform ptRect= pt.GetComponent<RectTransform>();
        ptRect.anchoredPosition = pos;
        ptRect.sizeDelta = new Vector2(2, 2);
        ptRect.anchorMin = new Vector2(0, 0.5f);
        ptRect.anchorMax = new Vector2(0, 0.5f);
    }

    public void ShowGraph(List<double> val){
        forces = new List<double>(val);
        count = val.Count;
        ymax = 1.1f * Math.Max( Math.Abs((float)val.Max()), Math.Abs((float)val.Min()) );
        Debug.Log("ymax: " + ymax);
        for (int i = 0; i < count; ++i)
        {
            float xPos =  ((float)(i) / (count-1)) * width;
            float yPos = (float)val[i] / ymax * height;
            PlotPoint(new Vector2(xPos, yPos), Color.white);
        }
    }

    public void ShowGraph(double[][] vals, int[] cluster) {
        
        //ymax = 1.1f * Math.Max( Math.Abs((float)(new List<double>(vals[1][0])).Max()), Math.Abs((float)vals.Min()) );
        double[] result = vals[0];
        for (int row = 1; row < vals.Length; row++) {
            for (int column = 0; column < vals[0].Length; column++) {
                if (vals[row][column] > result[column]) {
                    result[column] = vals[row][column];
                }
            }
        }
        //Debug.Log("ymax: " + ymax);*/
        for (int i = 0; i < vals.Length; ++i)
        {
            float xPos =  (float)vals[i][0] / (float)result[0] * width;
            float yPos = (float)vals[i][1] / ((float)result[1] * 2) * height;
            switch(cluster[i]){
                case 0: PlotPoint(new Vector2(xPos, yPos), Color.red);
                        valuesToWrite1.Add(new double[] {xPos, yPos});
                        break;
                case 1: PlotPoint(new Vector2(xPos, yPos), Color.green);
                        valuesToWrite2.Add(new double[] {xPos, yPos});
                        break;
                case 2: PlotPoint(new Vector2(xPos, yPos), Color.blue);
                        valuesToWrite3.Add(new double[] {xPos, yPos});
                        break;
                case 3: PlotPoint(new Vector2(xPos, yPos), Color.yellow);
                        valuesToWrite4.Add(new double[] {xPos, yPos});
                        break;
                case 4: PlotPoint(new Vector2(xPos, yPos), Color.cyan);
                        valuesToWrite4.Add(new double[] {xPos, yPos});
                        break;
                case 5: PlotPoint(new Vector2(xPos, yPos), Color.white);
                        valuesToWrite4.Add(new double[] {xPos, yPos});
                        break;
            }
        }

        /*StartCoroutine(WriteToCSV(valuesToWrite1, 1));
        StartCoroutine(WriteToCSV(valuesToWrite2, 2));
        StartCoroutine(WriteToCSV(valuesToWrite3, 3));
        StartCoroutine(WriteToCSV(valuesToWrite4, 4));*/
    }

    IEnumerator WriteToCSV(List<double[]> valuesToWrite, int no)
    {
    
        string ruta = UnityEngine.Application.dataPath + "/data " + no + ".csv";
    
        if (File.Exists(ruta))
        {
            File.Delete(ruta);
        }
    
        var sr = File.CreateText(ruta);
        
        for(int i = 0; i < valuesToWrite.Count; ++i)
            sr.WriteLine(valuesToWrite[i][0].ToString() + ", " + valuesToWrite[i][1].ToString());
    
        FileInfo fInfo = new FileInfo(ruta);
        fInfo.IsReadOnly = true;
    
        sr.Close();            
    
        yield return new WaitForSeconds(0.5f);
        
        UnityEngine.Application.OpenURL(ruta);
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