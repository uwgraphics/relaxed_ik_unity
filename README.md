# relaxed_ik_unity

This is the Unity wrapper of Relaxed IK.

## Run
1. Click play in Unity.
2. Hold left Alt key and use your mouse to navigate the camera
3. Use the GUI at the left side to play with the robot arm

## Steps of setting up a robot arm
1. Generate config files for the robot arm with relaxed_ik_config
   + One caveat: start configs in the info file should be floats (e.g., 0.0), otherwise Unity might crash
2. Follow the steps at [ros-sharp wiki](https://github.com/siemens/ros-sharp/wiki/User_App_NoROS_ImportURDFOnWindows) to set up the simulation of the robot arm
3. Once the robot arm is loaded, add the RelaxedIKUnity script to it. Set up the list of robot Limbs by dragging each link game object to the list
4. On the component of the UrdfRobot script, enable Is Kinematic and All Convex Colliders and disable Use Gravity and Use Inertia from URDF
5. Add a simple gripper by clicking Add Visual in the child called Visuals of the end effector game object. Add a Rigidbody and a collider component to the gripper. Enable Is Kinematic and disable Use Gravity in its Rigidbody component. (Alternative method: Drag and drop the Gripper prefab, be careful that the object structure should be `ee_name/Visuals/unnamed/Gripper`)
6. Set up the gripper transform in the RelaxedIKUnity component of the robot arm.
7. Add the name of the info file of the robot arm in `relaxed_ik_core/config/loaded_robot`
8. Optional: Add a child named `ViewCenter` tot the robot arm to configure the position of the camera at runtime

## Known Issues
1. In order to transform the orginal coordinate system in rviz to fit inside Unity, I write some hard code in RelaxedIKUnity.cs.
2. When the end effector is dragged to some point that the robot arm cannot reach (e.g. not long enough), EEPoseGoals go out of the range.
