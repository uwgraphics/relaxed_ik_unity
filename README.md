# Relaxed IK Unity

This wrapper allows users to use Relaxed IK in Unity on Windows. A few commonly used simulated robot arms have already been set up for you to play with. In the simulation, you will be able to disable or enable Relaxed IK as you like. When Relaxed IK is disabled, a joint angle writer panel will show up for you to adjust and visualize the joint angle configuration. When Relaxed IK is enabled, you will be able to have real-time interactions with Relaxed IK by dragging the transform gizmo associated with the gripper of the robot.

## Relaxed IK Family

More information about Relaxed IK, Collision IK, and all the wrappers could be found in this [documentation](https://uwgraphics.github.io/relaxed_ik_core/).

- [Relaxed IK (Deprecated)](https://github.com/uwgraphics/relaxed_ik/tree/dev)
- [Relaxed IK Core](https://github.com/uwgraphics/relaxed_ik_core)
- [Relaxed IK ROS1](https://github.com/uwgraphics/relaxed_ik_ros1)
- [Relaxed IK Unity](https://github.com/uwgraphics/relaxed_ik_unity)
- [Relaxed IK CoppeliaSim](https://github.com/uwgraphics/relaxed_ik_coppeliasim)
- [Relaxed IK Mujoco](https://github.com/uwgraphics/relaxed_ik_mujoco)

||**Relaxed IK (Deprecated)**|**Relaxed IK ROS1**|**Relaxed IK Unity**|**Relaxed IK Coppeliasim**|**Relaxed IK Mujoco**|  
|:------|:-----|:-----|:-----|:-----|:-----| 
|**Relaxed IK**|:o:|:o:|:o:|:o:|:o:|  
|**Collision IK**|:x:|:o:|:x:|:x:|:x:|  

## Dependencies

- To use this wrapper, you will first need to install Rust. Please go to https://www.rust-lang.org/learn/get-started for more infomation.
- You will also need to install Unity. Version 2020.3.5f1 or newer is recommended. An older version might or might not work dependending how old it is.

## Getting Started

1. Make sure that you have installed all the dependencies.
1. Clone this repo to your windows machine and open the subdirectory *RelaxedIK* as a Unity Project.
1. Initialize relaxed_ik_core (the Rust library of Relaxed IK) as a submodule by running the following command from the project directory:
    ```
    git submodule update --init
    ```
1. Navigate to the *relaxed_ik_core* folder and go through the steps below to get relaxed_ik_core ready.
    1. If your robot is in this list: [baxter, hubo, iiwa7, jaco7, panda, sawyer, ur5, yumi], ignore this step. Else, you will need to clone [this repo](https://github.com/uwgraphics/relaxed_ik) and follow the step-by-step guide [there](https://github.com/uwgraphics/relaxed_ik/blob/dev/src/start_here.py) to get the required robot config files into corresponding folders in the *config* folder in the core. To specify, there should be (replace "sawyer" with your robot name or your urdf name in some cases):
        - 1 self-collision file <collision_sawyer.yaml> in the *collision_files* folder
        - 4 Rust neural network files <sawyer_nn, sawyer_nn.yaml, sawyer_nn_jointpoint, sawyer_nn_jointpoint.yaml> in the *collision_nn_rust* folder
        - 1 info file <sawyer_info.yaml> in the *info_files* folder
        - 1 joint state function file <sawyer_joint_state_define> in the *joint_state_define_functions* folder
        - 1 urdf file <sawyer.urdf> in the *urdfs* folder.
    1. Look at <settings.yaml> in the *config* folder and follow the information there to customize the parameters.
    1. Compile the core:
        ```bash
        cargo build
        ```
1. If you are adding a new robot arm instead of working with an existing one, aside from generating robot config files in the last step, you also need to 
    1. Follow the steps at [ros-sharp wiki](https://github.com/siemens/ros-sharp/wiki/User_App_NoROS_ImportURDFOnWindows) to set up the simulation of the new robot arm in Unity. Please make sure that the urdf file has inertia information in it, otherwise the joints may not be set up correctly. 
    1. Once the robot arm is loaded, add the RelaxedIKUnity script in Assets/Scripts to it. Set up the list of robot Limbs and end-effectors by adding the corresponding game object to the list. The size of this list should be the same as the size of joint angle solution array published by RelaxedIK for that robot. You can refer to the corresponding <xxx_info.yaml> for how many and the order of links to add to the list. Be careful that you only want to add those links with the Hinge Joint component which imply that they are associated with revolute joints. 
    1. You also need to drag and drop the transform(s) of the end effector(s) and the pose goal prefab (which can be found at Assets/Prefabs/PoseGoal) to the Relaxed IK Unity component.
    1. On the component of the UrdfRobot script, enable Is Kinematic and All Convex Colliders and disable Use Gravity and Use Inertia from URDF.
    1. Optional: Add a child named ViewCenter to the robot arm to configure the position of the camera at runtime.

1. Copy and paste the new dll file which you can find in `relaxed_ik_core/target/debug/relaxed_ik_lib.dll` to `RelaxedIK/Assets/Plugins/relaxed_ik_lib.dll`.
1. Set the gameobject of a robot arm as active to play with it. Make sure only one robot arm is active at the same time.
1. Click play in Unity. Hold left Alt key and use your mouse to navigate the camera. Use the GUI on the left side to play with the robot arm. Have fun!

## Existing issues
- Lack of flexibility to customize the fixed frame of a robot. For example, in the case of hubo, we are not interested in the legs of hubo and would like to define its waist as the fixed frame.
- There is a hard-coded coordinate system transformation to sort out the difference of the coordinate system in Unity with that in Rviz in RelaxedIKUnity.cs.
The signature of that function is: `private Vector3 TransformUnityToRviz(int idx, Vector3 v)`.
- Collision IK is not set up in this wrapper. The key issue of setting it up would be communicating the environment representation in Unity with the Collision IK library.
