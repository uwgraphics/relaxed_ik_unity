# relaxed_ik_unity

This is the Unity wrapper of Relaxed IK.

## Run
1. Click play in Unity.

## Known Issues
1. In order to transform the orginal coordinate system in rviz to fit inside Unity, I write some hard code in RelaxedIKUnity.cs.
2. When the end effector is dragged to some point that the robot arm cannot reach (e.g. not long enough), EEPoseGoals go out of the range.
