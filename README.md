# Graphics-Lab-Spring-2020

## How to use the executable
1. Move free orbit camera using left and right mouse buttons.
2. The camera can be changed using the dropdown menu in the top left corner.
3. A point data file for the demonstration must be uploaded first before setting constraints.
4. The joint angles can be uploaded using a CSV file. The time scroll and the play button is located at the bottom.
5. Timestamps from ROS can be uploaded using a CSV file.
6. Before adding a constraint, make sure the current constraint is set to the appropriate constraint that is intended to be added.
7. The dropdown menu for the list of added constraints updates when a new one is added and the selected constraint can be removed using the 'remove' button.
8. Constraint information can be updated using the text fields and then added using the button, or can be uploaded as a CSV file which adds it automatically.
9. Images taken are saved at the application's path.

## File syntax conventions
1. Values inside CSV files must be separated by comma characters.
2. CSV files for tong and gripper force must have only one column pertaining to the force value.
3. CSV files for position must have 3 columns pertaining to the X, Y and Z values respectively, where Z is the up-vector.
4. CSV files for joint angles must have 7 columns pertaining to the joints from 0 to 6 in the same order.
5. CSV files for joint angles must be a single file that includes all joint angles from start to end of demonstration.
6. CSV files for constraint information must have the following in the same order and in a single row: wx, wy, wz, dx, dy, dz, radius. 'radius' value must be 0 if constraint is not axial.
7. CSV files for timestamps must have the following ROS time values in the same order and in a single row: safe compliant positioning, safe compliant replay 1, constraint interaction, safe compliant replay 2, end.

## Implementations in Unity
* Read this section after using the executable and along with the code for it to make complete sense.
### A. General information:
1. Since the Y axis is the up-vector in Unity and Z is the up-vector in the demonstration, the scripts use the Z values from the CSV files as Y and vice versa. Similar approach is used for rotations and angles. (Rotations can get messy and would probably require time to understand the implementation)
2. Standalone executable file uses a custom file browser which is included using SFB (Standalone File Browser) in the assembly reference.
3. Changing OS platforms would require changes to the file browser code and the message box code.
4. Some joints in the Sawyer model uses anti-clockwise as positive direction while others use clockwise as positive.
5. To implement the rotations correctly, the Sawyer model has child gameobjects (named "Body") in some joint objects to reset the rotations to zero for each joint.
6. The custom log handler only logs errors or warnings that are generated by the log() function inside scripts and will not catch any other runtime errors. This log file should only be used to debug the specific errors or warnings that occur inside these scripts.
7. Format for custom log file: Date Time, LogType, {newline}{tabspace}, GameObject(if applicable), ' - ', ScriptName, Message.
8. Implementation is optimized for the Sawyer robot. The UR5 model was implemented towards the end, hence there could be bugs in it.

### B. Pose selection algorithm:
1. Pose selection is currently implemented using a scoring system that scores each end-effector(gripper) position. The poses with the best scores are then selected for displaying.
2. The final scores have a lot of noise so they need to be smoothed. The smoothing is done using a sliding window median filter. After the noise is minimized, poses are selected using a sliding window that checks for local maximas inside that window. The local maximas are the desired gripper poses.
3. The current scoring system looks at distances between consecutive frames (samples) in the CSV file for position (longer distances means gripper has moved further, good points of interest), check if the gripper is moving slow (gripper moving slow could mean robot is doing something interesting), check if there is a big change in the direction end-effector is moving (larger change could mark points of interest).
4. There are two naive algorithms: select poses at a constant distance or time interval. The idea is that the new pose selection algorithm outputs a better summarized motion than these naive algorithms and is almost at par with the poses selected by hand.
#### Improvements that can be made:
1. Force data is not currently considered as force data for all test cases were not available. Force can be used as a parameter.
2. Pose selection is done by selecting the local maximas in the scores. The selection process can be experimented with to find a better selection process for the gripper poses using the scores.
3. Pose selection using K-means clustering was also experimented with. Scores closest to the cluster means were selected. Not much progress was made using this method, but further research may provide better results. Current implementation of Kmeans clustering method does not yeild good results. Could make improvements to it or remove the implementation.
4. Sudden change in rotation of the gripper may provide useful information. This can potentially be a new parameter.
5. The pose selection fails when there is repeated motion; poses keep getting selected and poses get merged with each other. There could be a system that detects repeated motion and summarizes that motion into one pose describing that movement (possibly using an arrow to represent the motion).
6. Poses in which there is occlusion or which merge together are selected, which is not ideal. The algorithm should only choose one gripper pose which does not collide or merge with other gripper poses.
7. The current algorithm does not consider the arms of the robot themselves. It does not display what the arms were doing at the specific gripper positions. This can be a parameter that can be considered and displaying the robot arms is also another option.

### C. Camera optimization:
1. Camera optimization in performed using a scoring method. Each camera view is scored by using a set of parameters that describe the view. The viewing angle with the highest score is then selected as this should, in theory, provide the best possible view of the motion.
2. There are three parameters currently considered in the algorithm: area of the trajectory seen on the screen, alignment of view with average gripper rotation, and occlusion of trajectory.
3. New parameters can be added to the scoring system to improve the algorithm.
4. The screen space that the trajectory occupies needs to be maximized (higher score is better).
5. Alignment of the camera with average gripper rotation uses a dot product (higher is better). The alignment is considered because, in theory, when the camera is perpendicular to the gripper there is more information that can be derived from the view.
6. The camera optimization script checks for occlusion of the end-effector trajectory by casting a ray to each point in the trajectory. If the trajectory is occluded then the score is zero and the other parameters do not matter. This is the only parameter that is binary. For the ray to be occluded, there needs to be a collider that represents the area that can be occluded. Currently, there is a collider attached to the robot that signifies the area that the robot can occlude.
7. The scene currently only contains a table. New objects can be added to the scene. For camera optimization to consider occlusion by the new object, add colliders to these objects.
#### Improvements that can be made:
1. Viewing angles at the eye-level may be better to look at. This can be added as a new parameter. Currently the camera optimization starts from the top, looking down at the constraint. 
2. Currently, alignment of the camera with the side of the gripper is only considered. The thought behind this was that viewing the gripper from the side would produce good results, but this fails when the trajectory is perpendicular to the gripper instead of parallel to the gripper (Ex: tracing.csv vs joint_angles_toaster.csv). Hence, in some cases, a view perpendicular and in front of the gripper may be preferred.
3. Viewing the path at a 30 or 20 degree angle may also be preferred. Looking at the path head on will not make the depth apparent.

### D. Forward Kinematics:
1. Forward Kinematics (FK) is used to plot the end-effector trajectory. A simple way to implement FK in Unity is to use child objects. The child follows the transform of its parent. So all that has to be done is to change the angles of the individual arms with respect to the transform of the parent. The joint angles must be in the angle between that joint and the previous joint. The joint angles in the CSV files are already in this format. Therefore, these values just need to be fed directly to the FK script.
2. FK in this project is implemented using a "skeleton" gameobject representing the robot's arms. This skeleton gameobject has cubes that are positioned at the centers of the joints. These cubes are not rendered in the scene as the mesh renderer component is disabled for them. Each cube represents one corresponding joint of the robot.
3. The joint angles are used to rotate the corresponding cubes. The final cube represents the end-effector point. The joint angles are looped through and the cubes are rotated. As this is being done, the end-effector points are used to plot the line for the trajectory using a line renderer component. The plotting is done inside the RobotControllerScript.cs script.
4. If a new robot is to be added and if FK is to be implemented then place the cubes in the joints of the robot arm. Each consecutive cube has to be the child of the one before so that the transform of that cube has the origin at its parent's. The rotations for the joints is taken care of inside the FK script. The transforms of each cube has to be rotated according to the joint angles. Figure out the axis on which the joint rotates when considering the cube. The cube has to be rotated around that axis. Us the negative signs to specify direction of rotation.