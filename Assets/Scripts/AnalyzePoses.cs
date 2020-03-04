using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyzePoses : MonoBehaviour
{
    private List<Vector3> endEff;
    private List<double> scores = new List<double>();
    private Vector3 prevDir = new Vector3();
    public List<double> CheckPoses(List<Vector3> endEff) {
        
        this.endEff = endEff;
        scores.Clear();
        scores.Add(0);

        for(int i = 500; i < endEff.Count; i++) {
            CalculateScore(endEff[i], i);
        }

        return scores;
    }
    public void CalculateScore(Vector3 point, int i) {
        
        double distance = Func.DistTo(point, endEff[i-1]);
        double movingSlow = 1 / (distance * 1000);
        
        float vx = endEff[i-2].x - point.x;
        float vy = endEff[i-2].y - point.y;
        float vz = endEff[i-2].z - point.z;

        float diff = Vector3.Dot(new Vector3(vx, vy, vz).normalized, prevDir.normalized);
        if(diff < 0.9)
            diff = 0;
        Debug.Log("diff = " + diff);


        prevDir.x = endEff[i-500].x - endEff[i-498].x;
        prevDir.y = endEff[i-500].y - endEff[i-498].y;
        prevDir.z = endEff[i-500].z - endEff[i-498].z;

        double currScore = distance*10000 + movingSlow - diff*10;

        Debug.Log(currScore);
        scores.Add(currScore);
    }
}
