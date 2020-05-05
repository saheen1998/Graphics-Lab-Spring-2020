using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using SFB;

public class UR5_ControllerScript : MonoBehaviour
{
    public Text precisionInput;
    public Text textDesc;
    public Transform tipTranform; //The tip (center) of the end-effector

    public Transform L0;
    public Transform L1;
    public Transform L2;
    public Transform L3;
    public Transform L4;
    public Transform L5;

    public Material mat_interact;
    public Material mat_normal;
    public GameObject gripperCorrectedModel; //For new algorithm
    public GameObject gripperModel; //For custom pose selection
    
    [HideInInspector] public Animation anim;
    
    private AnimationClip clip;
    private List<double> d0;
    private List<double> d1;
    private List<double> d2;
    private List<double> d3;
    private List<double> d4;
    private List<double> d5;
    
    private double tSafeComplPos = 2;
    private double tSafeComplReplay1 = 2;
    private double tConstraint = 2;
    private double tSafeComplReplay2 = 2;
    private double tEnd = 2;
    private int n_data;
    private ForwardKinematicsUR5 FKscr;
    private bool robotVisible = true;
    
    private GameObject gripper;
    private LineRenderer line; // Line to draw trajectory
    private Renderer rend;
    
    //Pose extraction
    public static List<Vector3> posePts = new List<Vector3>();
    public static List<GameObject> dupGrippers = new List<GameObject>();
    public GameObject positionText;
    private List<double> forces = new List<double>();
    [HideInInspector] public List<Vector3> endEffPos;
    private List<Quaternion> endEffRot;
    private AnalyzePosesUR5 AnalyzePosesScr;
    private List<double> poseScores;
    private int customGripperCount = 0;
    
    void Start()
    {
        FKscr = gameObject.GetComponent<ForwardKinematicsUR5>();
        gripper = GameObject.Find("Gripper");
        rend = gameObject.GetComponent<Renderer>();
        AnalyzePosesScr = gameObject.GetComponent<AnalyzePosesUR5>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Add animation in which arm rotates around x-axis
    void AddAnimX(string rPath, Transform arm, List<double> d, ref AnimationClip clip)
    {
        AnimationCurve xCurve;
        AnimationCurve yCurve;
        AnimationCurve zCurve;

        Keyframe[] xKeys = new Keyframe[n_data];
        Keyframe[] yKeys = new Keyframe[n_data];
        Keyframe[] zKeys = new Keyframe[n_data];

        float keyMultiplier = 10f/n_data;
        
        for (int i=0; i<n_data; i++)
        {
            xKeys[i] = new Keyframe(i*keyMultiplier, (float)d[i]*180/Mathf.PI);
            yKeys[i] = new Keyframe(i*keyMultiplier, arm.localEulerAngles.y);
            zKeys[i] = new Keyframe(i*keyMultiplier, arm.localEulerAngles.z);
        }

        xCurve = new AnimationCurve(xKeys);
        yCurve = new AnimationCurve(yKeys);
        zCurve = new AnimationCurve(zKeys);
        clip.SetCurve(rPath, typeof(Transform), "localEulerAngles.x", xCurve);
        clip.SetCurve(rPath, typeof(Transform), "localEulerAngles.y", yCurve);
        clip.SetCurve(rPath, typeof(Transform), "localEulerAngles.z", zCurve);
    }

    // Add animation in which arm rotates around y-axis
    void AddAnimY(string rPath, Transform arm, List<double> d, ref AnimationClip clip)
    {
        AnimationCurve xCurve;
        AnimationCurve yCurve;
        AnimationCurve zCurve;

        Keyframe[] xKeys = new Keyframe[n_data];
        Keyframe[] yKeys = new Keyframe[n_data];
        Keyframe[] zKeys = new Keyframe[n_data];

        float keyMultiplier = 10f/n_data;
        
        for (int i=0; i<n_data; i++)
        {
            xKeys[i] = new Keyframe(i*keyMultiplier, arm.localEulerAngles.x);
            yKeys[i] = new Keyframe(i*keyMultiplier, (float)d[i]*180/Mathf.PI);
            zKeys[i] = new Keyframe(i*keyMultiplier, arm.localEulerAngles.z);
        }

        xCurve = new AnimationCurve(xKeys);
        yCurve = new AnimationCurve(yKeys);
        zCurve = new AnimationCurve(zKeys);
        clip.SetCurve(rPath, typeof(Transform), "localEulerAngles.x", xCurve);
        clip.SetCurve(rPath, typeof(Transform), "localEulerAngles.y", yCurve);
        clip.SetCurve(rPath, typeof(Transform), "localEulerAngles.z", zCurve);
    }

    public void CreateRobotAnimation()
    {
		line = gameObject.GetComponent<LineRenderer>();
        endEffPos = new List<Vector3>();
        endEffRot = new List<Quaternion>();

        d0 = new List<double>();
        d1 = new List<double>();
        d2 = new List<double>();
        d3 = new List<double>();
        d4 = new List<double>();
        d5 = new List<double>();

        anim = GetComponent<Animation>();
        clip = new AnimationClip();
        clip.legacy = true;

        
		////////////Get each row from csv data file
		GameObject UIController = GameObject.Find("UI Controller");
		StreamReader joint_data;
        StreamReader temp;
		try{
			joint_data = new StreamReader(UIController.GetComponent<UI_Controller>().jointDataFilePath);
			temp = new StreamReader(UIController.GetComponent<UI_Controller>().jointDataFilePath);
		}catch{
            LogHandler.Logger.Log(gameObject.name + " - RobotControllerScript.cs: Joint data file not loaded, cannot be read or does not exist!", LogType.Warning);
			LogHandler.Logger.ShowMessage("Joint data file not loaded, cannot be read or does not exist!", "Warning!");
			return;
		}

        
        string tempdata;

        //Check if there are 6 joint angle value columns in the file
        tempdata = temp.ReadLine();
        string[] tempstr = tempdata.Split(new char[] {','} );
        if(tempstr.Length != 6){
            LogHandler.Logger.Log(gameObject.name + " - RobotControllerScript.cs: Not a joint data file with 6 joint data points for angles!", LogType.Error);
			LogHandler.Logger.ShowMessage("Not a joint data file with 6 joint data points for angles!", "Error!");
            return;
        }

        //Get number of lines in file
        int i = 1;
        do{
            tempdata = temp.ReadLine();
            i++;
        }while(tempdata != null);
        n_data = i - 1;

        //Read data from joint data file
		string data;
		data = joint_data.ReadLine();
        i = 0;
		line.positionCount = 0;
        do{
			string[] jointData = data.Split(new char[] {','} );

            // Add to the list containing joint angles for each arm
            // Negative sign indicates direction of rotation
            d0.Add(-double.Parse(jointData[0]));
            d1.Add(double.Parse(jointData[1]));
            d2.Add(double.Parse(jointData[2]));
            d3.Add(double.Parse(jointData[3]));
            d4.Add(-double.Parse(jointData[4]));
            d5.Add(-double.Parse(jointData[5]));

            // Used for plotting end-effector path using forward kinematics
            // FKscr is the forward kinematics script associated with the forward kinematics skeleton
            List<float> ang = new List<float>(){ (float)d0[i], (float)d1[i], (float)d2[i], (float)d3[i], (float)d4[i], (float)d5[i]};
            FKscr.ModifyPos(ang);
			line.SetPosition(line.positionCount++, FKscr.GetPoint());
            endEffPos.Add(FKscr.GetPoint());
            endEffRot.Add(FKscr.GetRotation());

            // Read next line of the CSV file
            i++;
			data = joint_data.ReadLine();
		}while(data != null);

        // Add animation using the joint angles read from the CSV file to each arm
        AddAnimY("world/base_link/shoulder_link", L0, d0, ref clip);
        AddAnimX("world/base_link/shoulder_link/upper_arm_link", L1, d1, ref clip);
        AddAnimX("world/base_link/shoulder_link/upper_arm_link/forearm_link", L2, d2, ref clip);
        AddAnimX("world/base_link/shoulder_link/upper_arm_link/forearm_link/wrist_1_link", L3, d3, ref clip);
        AddAnimY("world/base_link/shoulder_link/upper_arm_link/forearm_link/wrist_1_link/wrist_2_link", L4, d4, ref clip);
        AddAnimX("world/base_link/shoulder_link/upper_arm_link/forearm_link/wrist_1_link/wrist_2_link/wrist_3_link", L5, d5, ref clip);

        anim.AddClip(clip, "regular");
    }
    
    //////////////////******Pose extraction******//////////////////////////////////////

    // Get poses in a constant time interval
    public void ConstantTimeIntPoses() {
        
        poseScores = AnalyzePosesScr.ExtractPoseTimeInt(endEffPos);

        MakeGrippersForInt();
    }

    // Get poses in a constant distance interval
    public void ConstantDistIntPoses() {
        
        poseScores = AnalyzePosesScr.ExtractPoseDistInt(endEffPos);

        MakeGrippersForInt();
    }

    public void ClearAll() {

        foreach (GameObject dg in dupGrippers)
        {
            Destroy(dg);
        }
        posePts.Clear();
        posePts.Add(line.GetPosition(0));
        customGripperCount = 0;
    }
    
    // Funtion to start pose selection algorithm
    public void ShowScores() {

        // Get the scores for the poses
        // Forces not considered and implementation is commented out
        poseScores = AnalyzePosesScr.CheckPoses(endEffPos/*, forces*/);

        // Smooth the data using a sliding window median filter
        int winSize = 51;
        int hSize = winSize/2;
        List<double> tempScores = new List<double>(poseScores);
        for(int i = hSize; i < tempScores.Count-hSize; i++){
            poseScores[i] = Func.Median(tempScores, i-hSize, i+hSize);
        }

        MakeGrippers();
    }
    
    // Duplicate the gripper position for custom pose selection
    public void DuplicateGripper(){
        GameObject dGrip = Instantiate(gripperModel, gripper.transform.position, gripper.transform.rotation);
        dupGrippers.Add(dGrip);
        
        //Gripper Label Text
        GameObject pText = Instantiate(positionText, tipTranform.transform.position, Quaternion.identity);
        pText.GetComponent<TextMesh>().text = (customGripperCount + 1).ToString();
        customGripperCount++;
        pText.transform.SetParent(dGrip.transform);
    }

    // Make the grippers using the scoring algorithm
    private void MakeGrippers(){

        int count = 0;
        int winSize = Int32.Parse(precisionInput.text);
        int gripperCount = 0;
        
        for(int i = winSize/2; i < poseScores.Count-winSize/2; i++) {
            int flag = 1;
            /*if(poseScores[i] < 0)
                continue;*/
            for(int j = i-winSize/2; j < i+winSize/2; j++)
                if(poseScores[i] < poseScores[j]){
                    flag = 0;
                    break;
                }
            if(flag == 1){
                count++;
                //Debug.Log((i/(float)poseScores.Count) + " : " + poseScores[i]);
                GameObject dGrip = Instantiate(gripperCorrectedModel, endEffPos[i], endEffRot[i]);
                dupGrippers.Add(dGrip);

                //Gripper Label Text
                GameObject pText = Instantiate(positionText, dGrip.transform.position, Quaternion.identity);
                pText.GetComponent<TextMesh>().text = (gripperCount + 1).ToString();
                gripperCount++;
                pText.transform.SetParent(dGrip.transform);
            }
        }

        Debug.Log("Number of candidate poses: " + count);
    }

    // Used inside analyze poses script for Kmeans clustering
    public void MakeGrippers(int[] idxs) {
        for(int i = 0; i < idxs.Length; ++i) {
            Debug.Log("Making gripper " + i + "; Pos idx = " + idxs[i]);
            GameObject dGrip = Instantiate(gripperCorrectedModel, endEffPos[idxs[i]], endEffRot[idxs[i]]);
            dupGrippers.Add(dGrip);
        }
    }
    
    // Display the gripper positions for the naive algorithms (time and dist interval)
    public void MakeGrippersForInt(){

        int gripperCount = 0;

        for(int i = 0; i < poseScores.Count; i++) {
            if(poseScores[i] == 10) {
                GameObject dGrip = Instantiate(gripperCorrectedModel, endEffPos[i], endEffRot[i]);
                dupGrippers.Add(dGrip);
                
                //Gripper Label Text
                GameObject pText = Instantiate(positionText, dGrip.transform.position, Quaternion.identity);
                pText.GetComponent<TextMesh>().text = (gripperCount + 1).ToString();
                gripperCount++;
                pText.transform.SetParent(dGrip.transform);
            }
        }
    }

    public void OptimizeCamera() {
        GameObject.Find("Camera Optimize").GetComponent<OptimizeCameraAngle>().OptimizeCamera(endEffPos, endEffRot);
    }

    /////////////////************END*************//////////////////////////////////////
    

    /////////////////************UI Buttons*************//////////////////////////////
    public void ReadForceFile(){
        
        string forceFilePath;
        try{
            forceFilePath = StandaloneFileBrowser.OpenFilePanel("Open force data file", "", "csv", false)[0];
        }catch{
            return;
        }

		////////////Get each row from csv data file
		StreamReader forceD_data;
		try{
			forceD_data = new StreamReader(forceFilePath);
		}catch{
           	LogHandler.Logger.Log(gameObject.name + " - RobotControllerScript.cs: Demo force data file does not exist or cannot be read!", LogType.Error);
			LogHandler.Logger.ShowMessage("Demo force data file does not exist or cannot be read!", "Error!");
			return;
		}

        //Read demo data from joint data file
		string data;
		data = forceD_data.ReadLine();
        int count = 0;
        do{
			string[] FData = data.Split(new char[] {' '} );

            double val = double.Parse(FData[0]);
            if(val > 1)
                forces.Add(val);

            count++;
			data = forceD_data.ReadLine();
		}while(data != null);

        for(int i = 0; i < count; ++i) {
            float normTime = (float)i / count;
            if(normTime < tConstraint)
                forces.Insert(0, 0);
            else if(normTime >= tSafeComplReplay2)
                forces.Add(0);
        }
    }

    public void BrowseTimestampFile(){

        string timestampFilePath;

        try{
            timestampFilePath = StandaloneFileBrowser.OpenFilePanel("Open timestamp data file", "", "csv", false)[0];
        }catch{
            return;
        }

        StreamReader time_data;
        string data;
        try{
            time_data = new StreamReader(timestampFilePath);
            data = time_data.ReadLine();
            string[] tData = data.Split(new char[] {','} );
            tSafeComplPos = double.Parse(tData[0]);
            tSafeComplReplay1 = double.Parse(tData[1]);
            tConstraint = double.Parse(tData[2]);
            tSafeComplReplay2 = double.Parse(tData[3]);
            tEnd = double.Parse(tData[4]);
        }catch{
            LogHandler.Logger.Log(gameObject.name + " - RobotControllerScript.cs: Timestamp data file cannot be read or does not exist!", LogType.Warning);
            LogHandler.Logger.ShowMessage("Timestamp data file cannot be read or does not exist!", "Error!");
            return;
        }
        
        //Set normalized times for descriptor
        tEnd = tEnd - tSafeComplPos;
        tSafeComplReplay1 = (tSafeComplReplay1 - tSafeComplPos) / tEnd;
        tConstraint = (tConstraint - tSafeComplPos) / tEnd;
        tSafeComplReplay2 = (tSafeComplReplay2 - tSafeComplPos) / tEnd;
        tSafeComplPos = 0;
        tEnd = 0.99f;
    }

    public void play()
    {
        try{
            anim.Play("regular");
            anim["regular"].speed = 1;
            anim["regular"].normalizedTime = 0f;
        }catch{
            LogHandler.Logger.Log(gameObject.name + " - RobotControllerScript: Joint animation clip not found", LogType.Warning);
        }
    }

    public void animationScroll(float t)
    {
        try{
            anim.Play("regular");
            anim["regular"].speed = 0;
            anim["regular"].normalizedTime = t;

        }catch{
            LogHandler.Logger.Log(gameObject.name + " - RobotControllerScript: Joint animation clip not found!", LogType.Warning);
        }
    }

    public void ToggleRobot() {
        if (robotVisible) {
            transform.GetChild(0).gameObject.SetActive(false);
            robotVisible = false;
        } else {
            transform.GetChild(0).gameObject.SetActive(true);
            robotVisible = true;
        }
    }
}
