using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptimizeCameraAngle : MonoBehaviour
{
    //public List<Vector3> points;
    public Transform gripper;
    public Slider progressBar;
    public Text optCamButtonText;
    public Camera snapCam;

    private Bounds box;

    private bool startOptimization = false;
    private bool optimizedPos = false;
    private bool optCompleted = false;

    private float maxLenFromCenter = 0;
    private List<float> scores;
    private List<Vector3> points;
    private List<Quaternion> rotations;
    private Quaternion avgRot = new Quaternion();
    public Vector3 maxScoreCamPos;
    public float maxScore;
    private int angCount = 0;
    private int vertCount = 0; //Count number of downward or vertical rotations

    void Update()
    {
        if(startOptimization) {
            
            optCamButtonText.text = "Optimizing...";

            if(transform.position.y < maxLenFromCenter && optimizedPos == false)
                transform.Translate(0, 0, -0.01f);
            else {
                optimizedPos = true;
            }

            if(optimizedPos/* && scores.Count < 720*/) {
                transform.RotateAround(box.center, Vector3.up, 4);
                
                float minX = float.MaxValue;
                float maxX = float.MinValue;
                float minY = float.MaxValue;
                float maxY = float.MinValue;
                float occludeMult = 1;
                for(int i = 1; i < points.Count; ++i) {

                    RaycastHit hit = new RaycastHit();
                    Vector3 dir = (points[i] - transform.position).normalized;
                    if(Physics.Raycast(transform.position, dir, out hit, (float)Func.DistTo(transform.position, points[i])+0.05f))
                        if(hit.collider.gameObject.name == "Sawyer RobotController"/*hit.transform.root.gameObject.Equals("Sawyer RobotController")*/) {
                            occludeMult = 0;
                            //Debug.Log("Occluded");
                            Debug.DrawRay(transform.position, dir * hit.distance, Color.yellow);
                            break;
                        }

                    Vector3 screenPos = GetComponent<Camera>().WorldToScreenPoint(points[i]);
                    if(screenPos.x < minX)
                        minX = screenPos.x;
                    if(screenPos.x > maxX)
                        maxX = screenPos.x;
                        
                    if(screenPos.y < minY)
                        minY = screenPos.y;
                    if(screenPos.y > maxY)
                        maxY = screenPos.y;
                }

                float score = occludeMult * ( 10*((maxX - minX) / Screen.width) + ((maxY - minY) / Screen.height) + Mathf.Abs(Quaternion.Dot(transform.rotation, avgRot)) );
                
                //scores.Add(score);
                angCount += 4;
                //Debug.Log(angCount + " = " + score);

                if(score > maxScore) {
                    maxScore = score;
                    maxScoreCamPos = transform.position;
                }
            }

            float optimizationProgress = (vertCount*10 + angCount/36) / 120f;
            progressBar.value = optimizationProgress;

            if(angCount == 356) {
                angCount = 0;
                ++vertCount;
                transform.RotateAround(box.center, Vector3.right, -10);
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
            optCamButtonText.text = "Optimize Camera";
            StartCoroutine(TakePicture());
            optCompleted = false;
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
        optimizedPos = false;
        optCompleted = false;
        
        for(int i = 1; i < points.Count; ++i)
            box.Encapsulate(points[i]);

        Vector3 pos = box.center;
        transform.position = box.center;
        transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
        maxLenFromCenter = Mathf.Max(box.size.x, box.size.y, box.size.z);
        maxLenFromCenter = maxLenFromCenter * 2;

        //Calculate average rotation of all end-effectors
        float x = 0, y = 0, z = 0, w = 0;
        foreach (Quaternion q in rotations)
        {
            x += q.x; y += q.y; z += q.z; w += q.w;
        }
        float k = 1.0f / Mathf.Sqrt(x * x + y * y + z * z + w * w);
        avgRot = new Quaternion(x * k, y * k, z * k, w * k);// * new Quaternion(-0.7071068f, 0f, 0f, 0.7071068f);
        //Debug.Log(Quaternion.Dot(Quaternion.identity, new Quaternion(0.7071068f, 0f, 0f, 0.7071068f)));
        gripper.rotation = avgRot;// * new Quaternion(0.7071068f, 0f, 0f, 0.7071068f);

        startOptimization = true;
    }

    IEnumerator TakePicture() {
        yield return new WaitForSeconds(0.1f);
        Texture2D snapshot = new Texture2D(1024, 768, TextureFormat.RGB24, false);
        RenderTexture.active = snapCam.targetTexture;
        snapshot.ReadPixels(new Rect(0,0,1024,768), 0, 0);
        byte[] bytes = snapshot.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/Snapshot " + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png", bytes);
        Debug.Log(Application.dataPath);
    }
}
