using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptimizeCameraAngle : MonoBehaviour
{
    //public List<Vector3> points;
    public Transform gripper;

    private Bounds box;
    private bool startOptimization = false;
    private bool optimizedPos = false;
    private bool optCompleted = false;
    private bool startRot = false;
    private float maxLenFromCenter = 0;
    private List<float> scores;
    private List<Vector3> points;
    private List<Quaternion> rotations;
    public Vector3 maxScoreCamPos;
    public float maxScore;
    private int angCount = 0;
    private int vertCount = 0; //Count number of downward or vertical rotations

    void Update()
    {
        if(startOptimization) {
            
            if(transform.position.y < maxLenFromCenter && optimizedPos == false)
                transform.Translate(0, 0, -0.01f);
            else {
                optimizedPos = true;
            }

            if(optimizedPos/* && scores.Count < 720*/) {
                transform.RotateAround(box.center, Vector3.up, 1);
                
                float minX = float.MaxValue;
                float maxX = float.MinValue;
                float minY = float.MaxValue;
                float maxY = float.MinValue;
                Quaternion avgRot = rotations[0];
                for(int i = 1; i < points.Count; ++i) {
                    Vector3 screenPos = GetComponent<Camera>().WorldToScreenPoint(points[i]);
                    if(screenPos.x < minX)
                        minX = screenPos.x;
                    if(screenPos.x > maxX)
                        maxX = screenPos.x;
                        
                    if(screenPos.y < minY)
                        minY = screenPos.y;
                    if(screenPos.y > maxY)
                        maxY = screenPos.y;

                    avgRot = Quaternion.Lerp(avgRot, rotations[i], 0.5f);// * new Quaternion(0.5f, 0.5f, 0.5f, 0.5f);
                }

                float x = 0, y = 0, z = 0, w = 0;
                foreach (Quaternion q in rotations)
                {
                    x += q.x; y += q.y; z += q.z; w += q.w;
                }
                float k = 1.0f / Mathf.Sqrt(x * x + y * y + z * z + w * w);
                avgRot = new Quaternion(x * k, y * k, z * k, w * k);

                float score = ((maxX - minX) / Screen.width) + ((maxY - minY) / Screen.height) + Mathf.Abs(Quaternion.Dot(transform.rotation, avgRot)) - 1;
                gripper.rotation = avgRot;
                //scores.Add(score);
                ++angCount;
                Debug.Log(angCount + " = " + score);

                if(score > maxScore) {
                    maxScore = score;
                    maxScoreCamPos = transform.position;
                }
            }

            if(angCount == 359) {
                angCount = 0;
                ++vertCount;
                transform.RotateAround(box.center, Vector3.right, 10);
                transform.LookAt(box.center);
            }

            if(vertCount == 12) {
                startOptimization = false;
                optCompleted = true;
            }
        }

        if(optCompleted) {
            transform.position = maxScoreCamPos;
            transform.LookAt(box.center);
            /*transform.RotateAround(box.center, Vector3.up, idx/2);
            Debug.Log("Max score at " + idx + " = " + maxScore);
            optCompleted = false;*/
        }
    }

    public void OptimizeCamera(List<Vector3> newPoints, List<Quaternion> newRots) {
        points = newPoints;
        rotations = newRots;
        box = new Bounds(points[0], Vector3.zero);
        maxScore = float.MinValue;
        scores = new List<float>();
        angCount = 0;
        vertCount = 0;
        
        for(int i = 1; i < points.Count; ++i)
            box.Encapsulate(points[i]);

        Vector3 pos = box.center;
        transform.position = box.center;
        Debug.Log(points[0]);
        Debug.Log(box.center);
        transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
        maxLenFromCenter = Mathf.Max(box.size.x, box.size.y, box.size.z);
        maxLenFromCenter = maxLenFromCenter * 2;
        
        startOptimization = true;
    }
}
