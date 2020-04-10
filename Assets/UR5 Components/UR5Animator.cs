using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class UR5Animator : MonoBehaviour
{
    public Transform L0;
    public Transform L1;
    public Transform L2;
    public Transform L3;
    public Transform L4;
    public Transform L5;
    public Transform L6;
    
    [HideInInspector] public Animation anim;
    
    private AnimationClip clip;
    private List<double> d0;
    private List<double> d1;
    private List<double> d2;
    private List<double> d3;
    private List<double> d4;
    private List<double> d5;
    private List<double> d6;
    private int n_data;
    private LineRenderer line;
    private ForwardKinematicsUR5 FKscr;
    
    private List<Vector3> endEffPos = new List<Vector3>();
    private List<Quaternion> endEffRot = new List<Quaternion>();
    
    void Start()
    {
        FKscr = gameObject.GetComponent<ForwardKinematicsUR5>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

        d0 = new List<double>();
        d1 = new List<double>();
        d2 = new List<double>();
        d3 = new List<double>();
        d4 = new List<double>();
        d5 = new List<double>();
        d6 = new List<double>();

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

        //Check if there are 7 joint angle value columns in the file
        tempdata = temp.ReadLine();
        string[] tempstr = tempdata.Split(new char[] {','} );
        if(tempstr.Length != 7){
            LogHandler.Logger.Log(gameObject.name + " - RobotControllerScript.cs: Not a joint data file with 7 joint data points for angles!", LogType.Error);
			LogHandler.Logger.ShowMessage("Not a joint data file with 7 joint data points for angles!", "Error!");
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

            d0.Add(-double.Parse(jointData[0]));
            d1.Add(-double.Parse(jointData[1]));
            d2.Add(double.Parse(jointData[2]));
            d3.Add(-double.Parse(jointData[3]));
            d4.Add(double.Parse(jointData[4]));
            d5.Add(-double.Parse(jointData[5]));
            d6.Add(double.Parse(jointData[6]));

            List<float> ang = new List<float>(){ (float)d0[i], (float)d1[i], (float)d2[i], (float)d3[i], (float)d4[i], (float)d5[i], (float)d6[i]};
            FKscr.ModifyPos(ang);
			line.SetPosition(line.positionCount++, FKscr.GetPoint());
            endEffPos.Add(FKscr.GetPoint());
            endEffRot.Add(FKscr.GetRotation());

            i++;
			data = joint_data.ReadLine();
		}while(data != null);


        AddAnimY("world/base_link/shoulder_link", L0, d0, ref clip);
        AddAnimX("world/base_link/shoulder_link/upper_arm_link", L1, d1, ref clip);
        AddAnimX("world/base_link/shoulder_link/upper_arm_link/forearm_link", L2, d2, ref clip);
        AddAnimX("world/base_link/shoulder_link/upper_arm_link/forearm_link/wrist_1_link", L3, d3, ref clip);
        AddAnimY("world/base_link/shoulder_link/upper_arm_link/forearm_link/wrist_1_link/wrist_2_link", L4, d4, ref clip);
        AddAnimX("world/base_link/shoulder_link/upper_arm_link/forearm_link/wrist_1_link/wrist_2_link/wrist_3_link", L5, d5, ref clip);
        //AddAnim("base/L0/L1/Body/L2/Body/L3/Body/L4/Body/L5/Body/L6", L6, d6, ref clip);

        anim.AddClip(clip, "regular");
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
        /*
        foreach (GameObject dg in dupGrippers)
        {
            Destroy(dg);
        }
        posePts.Clear();
        posePts.Add(line.GetPosition(0));
        poseTimes.Clear();
        poseTimes.Add(0);
        pAng = 0;*/
    }

    public void ChangeAnimSpeed(float spd){
        try{
            anim["regular"].speed = spd;
        }catch{}
        /*animationSpeed = spd;
        textAnimSpeed.text = spd.ToString("F1");*/
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
}
