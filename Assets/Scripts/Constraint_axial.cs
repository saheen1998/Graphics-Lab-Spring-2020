using UnityEngine;
using System.Collections;
using System.IO;
using System.Windows.Forms;

public class Constraint_axial : MonoBehaviour {

	public Transform pointPrefab;
	public GameObject torusPrefab;

	private double wx, wy, wz, dx, dy, dz, radius;

	private Transform torusTransform;
	
	// Use this for initialization
	void Start () {
		
		double[] p = new double[3];
		LineRenderer line = gameObject.GetComponent<LineRenderer>();
		GameObject UIController = GameObject.Find("UI Controller");
		string data;
		
		//Get Constraint Information
		UI_Controller UIscr = UIController.GetComponent<UI_Controller>();
		if(UIscr.toBrowse){
			StreamReader constraint_data;
			try{
				constraint_data = new StreamReader(UIscr.constraintDataFilePath);
				data = constraint_data.ReadLine();
				string[] consData = data.Split(new char[] {','} );
				wx = double.Parse(consData[0]);
				wy = double.Parse(consData[1]);
				wz = double.Parse(consData[2]);
				dx = double.Parse(consData[3]);
				dy = double.Parse(consData[4]);
				dz = double.Parse(consData[5]);
				radius = double.Parse(consData[6]);
			}catch{
            	LogHandler.Logger.Log(gameObject.name + " - Constraint_axial.cs: Constraint data file cannot be read or does not exist!", LogType.Warning);
				LogHandler.Logger.ShowMessage("Constraint data file cannot be read or does not exist!", "Error!");
				return;
			}
		}else{
			try{
				wx = double.Parse(UIscr.info[0].text);
				wy = double.Parse(UIscr.info[1].text);
				wz = double.Parse(UIscr.info[2].text);
				dx = double.Parse(UIscr.info[3].text);
				dy = double.Parse(UIscr.info[4].text);
				dz = double.Parse(UIscr.info[5].text);
				radius = double.Parse(UIscr.info[6].text);
			}catch{
            	LogHandler.Logger.Log(gameObject.name + " - Constraint_axial.cs: Constraint input data is null or not in the correct format!", LogType.Warning);
			}
		}

		StreamReader xyz_data;
		try{
			xyz_data = new StreamReader(UIscr.pointDataFilePath);
		}catch{
            LogHandler.Logger.Log(gameObject.name + " - Constraint_axial.cs: Point data file cannot be read or does not exist!", LogType.Warning);
			LogHandler.Logger.ShowMessage("Point data file cannot be read or does not exist!", "Error!");
			return;
		}

		UIscr.AddConstriantInfo(wx, wy, wz, dx, dy, dz, radius, "Axial");
		//TextAsset xyz_data = Resources.Load<TextAsset>(dataFileName);
		//string[] data = xyz_data.text.Split(new char[] {'\n'} );

		//Get each row from csv data file
		data = xyz_data.ReadLine();
		line.positionCount = 0;
		//Plot each point
		do{
			string[] pointData = data.Split(new char[] {','} );
			p[0] = double.Parse(pointData[0]);
			p[1] = double.Parse(pointData[2]);
			p[2] = double.Parse(pointData[1]);
			transform.position = new Vector3((float)p[0], (float)p[1], (float)p[2]);
			var point = Instantiate(pointPrefab, transform.position, Quaternion.identity);
			point.name = "Point " + UIscr.selectedConsIdx.ToString();

			//Set a vertex for the line at the point
			line.SetPosition(line.positionCount++, transform.position);

			//Check if point is on constraint
			//int resConstraint = Func.check_constraint_axial(wx, wy, wz, dx, dy, dz, radius, p);
			
			data = xyz_data.ReadLine();
		}while(data != null);

		//Move constraint center to centroid
		
		double[,] w = {{-wx},{wz},{-wy}};
		double[,] ew = new double[3, 3];
		float qx = 0, qy = 0, qz = 0, qw = 0;

		ew = Func.exp_map(w);
		Func.matToQ(ew, ref qx, ref qy, ref qz, ref qw);

		Quaternion rot = new Quaternion(qx, qy, qz, qw);
		GameObject obj_axialTorus;
		Vector3 center = new Vector3((float)dx, (float)dz, (float)dy);
		obj_axialTorus = /*(GameObject)*/Instantiate(torusPrefab, center, rot);
		obj_axialTorus.name = "Constraint " + UIscr.selectedConsIdx.ToString();
		ParticleSystem.ShapeModule ring = obj_axialTorus.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().shape;
		ring.radius = Mathf.Abs((float)radius);
	}
}
