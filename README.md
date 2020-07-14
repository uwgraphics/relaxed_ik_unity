# relaxed_ik_unity
This is the Unity wrapper of Relaxed IK.

## Run
1. Click play in Unity.
2. Hold left Alt key and use your mouse to navigate the camera.
3. Use the GUI at the left side to play with the robot arm.
4. Set a robot arm as active to play with it. Make sure only one robot arm is active at the same time.

## Steps of setting up a robot arm
1. Generate config files for the robot arm with relaxed_ik_config.
   + Start configs in the info file should be floats (e.g., 0.0), otherwise Unity might crash.
2. Follow the steps at [ros-sharp wiki](https://github.com/siemens/ros-sharp/wiki/User_App_NoROS_ImportURDFOnWindows) to set up the simulation of the robot arm.
   + Please make sure that the urdf file has inertia information in it, otherwise the joints may not be set up correctly.
3. Once the robot arm is loaded, add the RelaxedIKUnity script to it. Set up the list of robot Limbs and end effectors by dragging and dropping the corresponding game object to the list. The gripper prefab can be found at `Assets/Prefabs/Gripper`.
4. On the component of the UrdfRobot script, enable Is Kinematic and All Convex Colliders and disable Use Gravity and Use Inertia from URDF.
5. Optional: Add a child named `ViewCenter` tot the robot arm to configure the position of the camera at runtime.

## Known Issues
1. In order to transform the orginal coordinate system in rviz to fit inside Unity, I write some hard code in RelaxedIKUnity.cs.
2. When the end effector is dragged to some point that the robot arm cannot reach (e.g. not long enough), EEPoseGoals go out of the range.
3. For the sawyer robot arm, dragging the gripper in the x (or y) axis will lead to changes in the y (or x) coordinate of the pose goal.
4. For yumi and panda, relaxed IK doesn't run very smoothly, might be the problem of their config files
