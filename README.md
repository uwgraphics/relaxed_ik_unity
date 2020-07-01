# relaxed_ik_unity

This is the Unity wrapper of Relaxed IK.

## Run
1. Click play in Unity.

## Steps of setting up iiwa7
1. Generate config files for iiwa7 with relaxed_ik_config
2. Follow the steps at [ros-sharp wiki](https://github.com/siemens/ros-sharp/wiki/User_App_NoROS_ImportURDFOnWindows) to set up the simulation of the robot arm
3. Once the robot arm is loaded, add the RelaxedIKUnity script to it. Set up the list of robot Limbs by dragging each link game object to the list
4. On the component of the UrdfRobot script, enable Is Kinematic and All Convex Colliders and disable Use Gravity and Use Inertia from URDF
5. Add a simple gripper by clicking Add Visual in the child called Visuals of the end effector game object. Add a Rigidbody and a collider component to the gripper. Enable Is Kinematic and disable Use Gravity in its Rigidbody component
6. Set up the gripper transform in the RelaxedIKUnity component of the robot arm.
7. Add the name of the info file of the robot arm in `relaxed_ik_core/config/loaded_robot`

## Known Issues
1. In order to transform the orginal coordinate system in rviz to fit inside Unity, I write some hard code in RelaxedIKUnity.cs.
2. When the end effector is dragged to some point that the robot arm cannot reach (e.g. not long enough), EEPoseGoals go out of the range.
