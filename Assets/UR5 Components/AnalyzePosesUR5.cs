using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnalyzePosesUR5 : MonoBehaviour
{
    public Text timeIntInput;
    public Text distIntInput;
    public KmeansClustering kmeansScr;
    public UR5_ControllerScript rbcScr;

    private List<Vector3> endEff;
    private List<double> scores = new List<double>();
    private Vector3 prevDir = new Vector3();

    private List<double> dists = new List<double>();
    private List<double> moveSlows = new List<double>();
    private List<double> sameDirs = new List<double>();
    
    /* Main function used to analyze poses and score each pose
        NOT IMLEMENTED: Forces from tongs not considered. Implementation is commented out
    */
    public List<double> CheckPoses(List<Vector3> endEff/*, List<double> forces*/) {
        
        this.endEff = endEff;
        scores.Clear();
        scores.Add(0);

        //Commented out section is used for considering force as a parameter
        for(int i = 10; i < endEff.Count; i++) {
            //float idx = ((float)i/endEff.Count) * forces.Count;
            CalculateScore(endEff[i], i, 0/*forces[(int)idx]*/);
        }

        return scores;
    }

    /* Function that calls Kmeans clustering script
        *This function is called on a button press on the canvas
    */
    public void StartKmeans() {
        int[] pts = kmeansScr.StartClustering(dists, moveSlows, sameDirs, 5);
        rbcScr.MakeGrippers(pts);
    }

    /*Get end effector poses in a constant time interval
        *Set the score of poses after interval as 10 and 0 for every other poses
    */
    public List<double> ExtractPoseTimeInt(List<Vector3> endEff) {
        
        this.endEff = endEff;
        scores.Clear();
        scores.Add(0);

        //Interval for samples in CSV
        int interval = Int32.Parse(timeIntInput.text);

        for(int i = 0; i < endEff.Count; i++) {
            if(i % interval == 0)
                scores.Add(10);
            else
                scores.Add(0);
        }
        return scores;
    }

    /* Get end effector poses in a constant distance interval
        *Set the score of poses after interval as 10 and 0 for every other poses
    */
    public List<double> ExtractPoseDistInt(List<Vector3> endEff) {
        
        this.endEff = endEff;
        scores.Clear();
        scores.Add(0);

        double dist = 0;
        float interval = float.Parse(distIntInput.text);
        for(int i = 1; i < endEff.Count; i++) {
            dist += Func.DistTo(endEff[i-1], endEff[i]);
            Debug.Log(dist);
            if(dist > interval) {
                scores.Add(10);
                dist = 0;
            }
            else
                scores.Add(0);
        }
        return scores;
    }

    /* Custom algorithm for selecting poses
        *Parameters
            -distance: Distance between consecutive poses
            -movingSlow: Check if end-effector is moving slow
            -sameDir: Check if there is a big change in the direction end-effector is moving
    */
    private void CalculateScore(Vector3 point, int i, double force) {
        
        double distance = Func.DistTo(point, endEff[i-1]);
        double movingSlow;
        movingSlow = 1 / (distance * 1000);
        movingSlow = (double)Mathf.Clamp((float)movingSlow, 0.1f, 10);
        
        float vx = endEff[i-2].x - point.x;
        float vy = endEff[i-2].y - point.y;
        float vz = endEff[i-2].z - point.z;

        float sameDir = Vector3.Dot(new Vector3(vx, vy, vz).normalized, prevDir.normalized);


        prevDir.x = endEff[i-10].x - endEff[i-9].x;
        prevDir.y = endEff[i-10].y - endEff[i-9].y;
        prevDir.z = endEff[i-10].z - endEff[i-9].z;

        double currScore = distance*10000 + movingSlow - sameDir*10 + force;

        dists.Add(distance*1000);
        moveSlows.Add(movingSlow);
        sameDirs.Add(sameDir);

        //Debug.Log(distance*1000 + ", " + movingSlow + ", " + sameDir);

        //Debug.Log(currScore);
        scores.Add(currScore);
    }
}
