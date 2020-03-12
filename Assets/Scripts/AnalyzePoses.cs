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

        for(int i = 10; i < endEff.Count; i++) {
            CalculateScore(endEff[i], i);
        }

        return scores;
    }

    //Constant time interval
    public List<double> ExtractPoseTimeInt(List<Vector3> endEff) {
        
        this.endEff = endEff;
        scores.Clear();
        scores.Add(0);

        for(int i = 0; i < endEff.Count; i++) {
            if(i % 200 == 0)
                scores.Add(10);
            else
                scores.Add(0);
        }
        return scores;
    }

    //Constant distance interval
    public List<double> ExtractPoseDistInt(List<Vector3> endEff) {
        
        this.endEff = endEff;
        scores.Clear();
        scores.Add(0);

        double dist = 0;

        for(int i = 1; i < endEff.Count; i++) {
            dist += Func.DistTo(endEff[i-1], endEff[i]);
            Debug.Log(dist);
            if(dist > 0.2) {
                scores.Add(10);
                dist = 0;
            }
            else
                scores.Add(0);
        }
        return scores;
    }

    public void CalculateScore(Vector3 point, int i) {
        
        double distance = Func.DistTo(point, endEff[i-1]);
        double movingSlow;
        if(distance < 0.01)
            movingSlow = 0.1;
        else
            movingSlow = 1 / (distance * 1000);
        
        float vx = endEff[i-2].x - point.x;
        float vy = endEff[i-2].y - point.y;
        float vz = endEff[i-2].z - point.z;

        float sameDir = Vector3.Dot(new Vector3(vx, vy, vz).normalized, prevDir.normalized);


        prevDir.x = endEff[i-10].x - endEff[i-9].x;
        prevDir.y = endEff[i-10].y - endEff[i-9].y;
        prevDir.z = endEff[i-10].z - endEff[i-9].z;

        double currScore = distance*10000 + movingSlow - sameDir*10;

        Debug.Log(currScore);
        scores.Add(currScore);
    }
}
