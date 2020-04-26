using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;
using SFB;

public class UI_Controller : MonoBehaviour
{
    public GameObject menu;
    public GameObject poseMenu;
    public GameObject ImageSavedAlert;
    public List<GameObject> cameras;
    public List<GameObject> options;
    public List<GameObject> penPrefabs;
    public GameObject inputFields;
    public List<InputField> info;
    public GameObject constraintList;
    public Material matNormal;
    public Material matSelected;
    public Camera snapCam;

    //Variables used inside script only//////////////
    [HideInInspector] public bool toBrowse;
    // Constraint data
    [HideInInspector] public List<double> wx;
    [HideInInspector] public List<double> wy;
    [HideInInspector] public List<double> wz;
    [HideInInspector] public List<double> dx;
    [HideInInspector] public List<double> dy;
    [HideInInspector] public List<double> dz;
    [HideInInspector] public List<double> rad;

    public double demoSafeRep1;
    public double demoConstraint;
    public double demoSafeRep2;
    public double demoEnd;
    [HideInInspector] public string pointDataFilePath;
    [HideInInspector] public string jointDataFilePath;
    [HideInInspector] public string constraintDataFilePath;

    private Dropdown dropdownConstraintList;
    [HideInInspector] public int selectedConsIdx = 0;
    private int constraintInd = 0;
    
    // Change the camera
    // cameras[1] always needs to be active it is the camera used for optimization
    // Optimization camera has a depth of -1 so that when 'free orbit' camera (depth of 0) is active the screen switches to that camera
    public void ChangeCamera(int index)
    {
        foreach (GameObject cam in cameras)
        {
            cam.SetActive(false);
        }
        cameras[index].SetActive(true);
        cameras[1].SetActive(true);
    }

    public void ToggleOptions(){
        foreach (GameObject opt in options)
        {
            opt.SetActive((opt.activeSelf) ? (false) : (true));
        }
    }
    
    public void ToggleMenu(){
        poseMenu.SetActive(false);
        menu.SetActive((menu.activeSelf) ? (false) : (true));
    }

    public void TogglePoseMenu(){
        menu.SetActive(false);
        poseMenu.SetActive((poseMenu.activeSelf) ? (false) : (true));
    }

    ///////////////////Functions for constraint////////////
    public void SetConstraint(int ind){
        constraintInd = ind;
    }

    public void AddConstriantInfo(double w1, double w2, double w3, double d1, double d2, double d3, double r, string cons){

        wx.Add(w1);
        wy.Add(w2);
        wz.Add(w3);
        dx.Add(d1);
        dy.Add(d2);
        dz.Add(d3);
        rad.Add(r);

        Dropdown.OptionData opt = new Dropdown.OptionData();
        opt.text = (dropdownConstraintList.options.Count + 1).ToString() + ". " + cons;
        dropdownConstraintList.options.Add(opt);
        selectedConsIdx = dropdownConstraintList.options.Count - 1;
        dropdownConstraintList.value = selectedConsIdx + 1;
    }

    public void SelectConstraint(int idx){

        info[0].text = wx[idx].ToString();
        info[1].text = wy[idx].ToString();
        info[2].text = wz[idx].ToString();
        info[3].text = dx[idx].ToString();
        info[4].text = dy[idx].ToString();
        info[5].text = dz[idx].ToString();
        info[6].text = rad[idx].ToString();
        selectedConsIdx = idx;

        //Change color to indicate selected constraint (Not Implemented)
        /*GameObject[] constraints = GameObject.FindGameObjectsWithTag("Constraint");
        foreach (GameObject c in constraints)
        {
            if(c.name == ("Constraint " + selectedConsIdx))
                c.GetComponent<MeshRenderer>().material = matSelected;
            else
                c.GetComponent<MeshRenderer>().material = matNormal;
        }*/
    }

    // Remove the constraint by removing it from dropdown list and saved constraints data
    public void RemoveConstraint(){
        string cText = dropdownConstraintList.options[dropdownConstraintList.value].text;
        string[] cNum = cText.Split(new char[] {'.'} );
        selectedConsIdx = int.Parse(cNum[0]) - 1;
        Destroy(GameObject.Find("Pen " + selectedConsIdx));
        Destroy(GameObject.Find("Constraint " + selectedConsIdx));
        GameObject[] points = GameObject.FindGameObjectsWithTag("Point");
        foreach (GameObject pt in points){
            if(pt.name == "Point " + selectedConsIdx)
                Destroy(pt);
        }

        // Remove the constraint data associated with the selected constraint
        wx.RemoveAt(dropdownConstraintList.value);
        wy.RemoveAt(dropdownConstraintList.value);
        wz.RemoveAt(dropdownConstraintList.value);
        dx.RemoveAt(dropdownConstraintList.value);
        dy.RemoveAt(dropdownConstraintList.value);
        dz.RemoveAt(dropdownConstraintList.value);
        rad.RemoveAt(dropdownConstraintList.value);

        // Remove constraint from the dropdown list
        dropdownConstraintList.options.RemoveAt(dropdownConstraintList.value);
        if(dropdownConstraintList.options.Count <= 0)
            constraintList.transform.GetChild(0).GetComponent<Text>().text = "";
        else
            dropdownConstraintList.value = dropdownConstraintList.options.Count - 1;
    }

    public void ToggleConstraintInfo(){
        inputFields.SetActive((inputFields.activeSelf) ? (false) : (true));
    }

    public void AddConstraint(){

        toBrowse = false;

        selectedConsIdx = dropdownConstraintList.options.Count;
        GameObject pen = Instantiate(penPrefabs[constraintInd]);
        pen.name = "Pen " + (selectedConsIdx).ToString();
        
        GameObject cam = GameObject.Find("CamConstraint");
        if(cam != null) cam.GetComponent<CamConstraint>().ResetPos();
    }

    public void browseConstraintInfoFile(){
        try{
            constraintDataFilePath = StandaloneFileBrowser.OpenFilePanel("Open constraint information data file", "", "csv", false)[0];
            toBrowse = true;

            selectedConsIdx = dropdownConstraintList.options.Count;

            GameObject pen = Instantiate(penPrefabs[constraintInd]);
            pen.name = "Pen " + (selectedConsIdx).ToString();
            
            GameObject cam = GameObject.Find("CamConstraint");
            if(cam != null) cam.GetComponent<CamConstraint>().ResetPos();
        }catch{}
    }

    public void ShowConstraintList(){
        constraintList.SetActive((constraintList.activeSelf) ? (false) : (true));
    }
    //////////////////////********END*************////////////////////////////

    public void SetPointDataFile(){
        try{
            pointDataFilePath = StandaloneFileBrowser.OpenFilePanel("Open point data file", "", "csv", false)[0];
        }catch{}
    }

    public void SetJointDataFile(){
        try{
            jointDataFilePath = StandaloneFileBrowser.OpenFilePanel("Open joint data file", "", "csv", false)[0];
        }catch{}
    }

    public void TakePicture() {
        Texture2D snapshot = new Texture2D(1024, 768, TextureFormat.RGB24, false);
        RenderTexture.active = snapCam.targetTexture;
        snapshot.ReadPixels(new Rect(0,0,1024,768), 0, 0);
        byte[] bytes = snapshot.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/Snapshot " + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png", bytes);
        //Debug.Log(Application.dataPath);
        ImageSavedAlert.GetComponent<ImageSaved>().activate();
    }

    private void Start() {
        dropdownConstraintList = constraintList.GetComponent<Dropdown>();
    }
}
