using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptimizeCameraAngle : MonoBehaviour
{
    public GameObject ImageSavedAlert;
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
        // Optimization is done in real-time by scoring each camera position
        // Modify the camera position and score it in each frame to reduce frame rate drop and to make implementations easier
        // All possible angles are not checked as it takes a long time. Angles are skipped in small intervals to speed up as well as minimize the risk of losing potentially good viewing angles
        if(startOptimization) {
            
            optCamButtonText.text = "Optimizing...";

            // Zoom the camera out from the center of bounding box so all points are visible
            if(transform.position.y < maxLenFromCenter && optimizedPos == false)
                transform.Translate(0, 0, -0.01f);
            else {
                optimizedPos = true;
            }

            // Actual optimization starts by rotating the camera around the end-effector path and scoring each viewing angle
            if(optimizedPos) {

                // Rotate the camera around the bounding box center for new viewing angle
                // 4 degrees is chosen to maximize speed and efficiency
                transform.RotateAround(box.center, Vector3.up, 4);
                
                float minX = float.MaxValue;
                float maxX = float.MinValue;
                float minY = float.MaxValue;
                float maxY = float.MinValue;
                float occludeMult = 1;
                // Loop to check all points in the end-effector path
                for(int i = 1; i < points.Count; ++i) {

                    // Check if camera view angle is occluded by the robot
                    // The robot has a collider attached which specifies the area behind the robot the camera needs to omit
                    // If view is occluded then skip the for loop
                    RaycastHit hit = new RaycastHit();
                    Vector3 dir = (points[i] - transform.position).normalized;
                    if(Physics.Raycast(transform.position, dir, out hit, (float)Func.DistTo(transform.position, points[i])+0.05f))
                        if(hit.collider.gameObject.name == "Sawyer RobotController"/*hit.transform.root.gameObject.Equals("Sawyer RobotController")*/) {
                            occludeMult = 0;
                            //Debug.Log("Occluded");
                            Debug.DrawRay(transform.position, dir * hit.distance, Color.yellow);
                            break;
                        }

                    // Find the area on the screen on which the end-effector points map to
                    // Get the rectangle on the screen which encapsulates all points by finding the left-most, right-most, top-most, and bottom-most points
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
                /* Score the view angle
                    *occludeMult: if view is occluded then make the score 0
                    *Find the percentage of the screen space occupied by the bounding rectangle on the screen
                    *Dot product to check whether the camera aligns with the average end-effector rotation
                */
                float score = occludeMult * ( 10*((maxX - minX) / Screen.width) + ((maxY - minY) / Screen.height) + Mathf.Abs(Quaternion.Dot(transform.rotation, avgRot)) );
                
                // Increase the count by 4 to specify the degrees of angles checked
                angCount += 4;
                //Debug.Log(angCount + " = " + score);

                // Save the camera position with the highest score
                if(score > maxScore) {
                    maxScore = score;
                    maxScoreCamPos = transform.position;
                }
            }

            // Progress of the optimization between 0 and 1 to feed into the slider which acts as the progress bar
            float optimizationProgress = (vertCount*10 + angCount/36) / 120f;
            progressBar.value = optimizationProgress;

            // If we have reached 356 degrees on the y-axis then move to a 10 degree lower viewing angle and repeat again
            if(angCount == 356) {
                angCount = 0;
                ++vertCount;
                transform.RotateAround(box.center, Vector3.right, -10);
                transform.LookAt(box.center);
            }

            //If viewing angle has been lowered 12 times then all potentially good viewing angles have been checked. Optimization completed
            if(vertCount == 12) {
                startOptimization = false;
                optCompleted = true;
            }
        }

        // Optimization is complete; move the camera to the position with highest score
        if(optCompleted) {
            transform.position = maxScoreCamPos;
            transform.LookAt(box.center);
            optCamButtonText.text = "Optimize Camera";
            StartCoroutine(TakePicture());
            optCompleted = false;
        }
    }

    public void OptimizeCamera(List<Vector3> newPoints, List<Quaternion> newRots) {
        points = newPoints;
        rotations = newRots;
        maxScore = float.MinValue;
        scores = new List<float>();
        angCount = 0;
        vertCount = 0;
        optimizedPos = false;
        optCompleted = false;
        
        // Encapsulate all end-effector points in a bounding box
        box = new Bounds(points[0], Vector3.zero);
        for(int i = 1; i < points.Count; ++i)
            box.Encapsulate(points[i]);

        // Align the camera with the center of the bounding box
        transform.position = box.center;
        transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
        //maxLenFromCenter is the distance the camera has to be kept for it to see all points from any angle
        maxLenFromCenter = Mathf.Max(box.size.x, box.size.y, box.size.z);
        maxLenFromCenter = maxLenFromCenter * 2;

        //Calculate average rotation of all end-effector (gripper) positions
        float x = 0, y = 0, z = 0, w = 0;
        foreach (Quaternion q in rotations)
        {
            x += q.x; y += q.y; z += q.z; w += q.w;
        }
        float k = 1.0f / Mathf.Sqrt(x * x + y * y + z * z + w * w);
        avgRot = new Quaternion(x * k, y * k, z * k, w * k);
        gripper.rotation = avgRot;

        // Start optimizing camera
        startOptimization = true;
    }

    IEnumerator TakePicture() {
        yield return new WaitForSeconds(0.1f);
        Texture2D snapshot = new Texture2D(1024, 768, TextureFormat.RGB24, false);
        RenderTexture.active = snapCam.targetTexture;
        snapshot.ReadPixels(new Rect(0,0,1024,768), 0, 0);
        byte[] bytes = snapshot.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/Snapshot " + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png", bytes);
        ImageSavedAlert.GetComponent<ImageSaved>().activate();
    }
}
