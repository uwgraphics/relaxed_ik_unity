using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeGizmos;

namespace RosSharp.RosBridgeClient
{
    public class RelaxedIKUnity : MonoBehaviour
    {
        public bool enableRelaxedIK;
        public List<GameObject> robotLinks;
        public List<Transform> grippers;
        public Opt xopt;
        /*public double[] posTest = new double[] { 0.015, 0.015, 0.015 };
        public double[] quatTest = new double[] { 0.0, 0.0, 0.0, 1.0 };*/

        private List<Joint> joints;
        private List<Quaternion> baseRotations;
        private List<Vector3> gripperPos;
        private List<Quaternion> gripperQuat;
        // private Transform root;
        private List<TransformGizmo> gizmos;

        // Start is called before the first frame update
        private void Start()
        {
            joints = new List<Joint>();
            baseRotations = new List<Quaternion>();
            // root = transform.root;

            gripperPos = new List<Vector3>();
            gripperQuat = new List<Quaternion>();

            for (int i = 0; i < grippers.Count; i++) {
                gripperPos.Add(grippers[i].localPosition);
                gripperQuat.Add(grippers[i].localRotation);
            }
            

            foreach (GameObject link in robotLinks)
            {
                Joint joint = link.GetComponent<Joint>();
                // if (joint.axis.y == 1)
                // {
                //     joint.axis = new Vector3(0,-1,0);
                // }
                joints.Add(joint);
                baseRotations.Add(link.transform.localRotation);
            }

            gizmos = new List<TransformGizmo>(FindObjectsOfType(typeof(TransformGizmo)) as TransformGizmo[]);
        }

        private unsafe void Update()
        {
            if (enableRelaxedIK) 
            {
                foreach (TransformGizmo gizmo in gizmos)
                    gizmo.enabled = true;
                double[] posArr = new double[3 * grippers.Count];
                double[] quatArr = new double[4 * grippers.Count];
                for (int i = 0; i < grippers.Count; i++) {
                    gripperPos[i] += TransformUnityToRvizPos(grippers[i].localPosition);
                    gripperQuat[i] *= TransformUnityToRvizRot(grippers[i].localRotation);
                    posArr[3 * i] = gripperPos[i].x;
                    posArr[3 * i + 1] = gripperPos[i].y;
                    posArr[3 * i + 2] = gripperPos[i].z;
                    quatArr[4 * i] = gripperQuat[i].x;
                    quatArr[4 * i + 1] = gripperQuat[i].y;
                    quatArr[4 * i + 2] = gripperQuat[i].z;
                    quatArr[4 * i + 3] = gripperQuat[i].w;

                    // Debug.Log(gripperPos[i]);
                    // Debug.Log(gripperQuat[i]);
                }
                
                xopt = RelaxedIKLoader.runUnity(posArr, posArr.Length, quatArr, quatArr.Length);
                
                string jaStr = "";
                for (int i = 0; i < xopt.length; i++)
                {
                    float angle = Mathf.Rad2Deg * (float) xopt.data[i];
                    Vector3 axis = angle * joints[i].axis;
                    robotLinks[i].transform.localEulerAngles = (baseRotations[i] * Quaternion.Euler(axis)).eulerAngles;
                    jaStr += i == 0 ? "[" + xopt.data[i].ToString() : ", " + xopt.data[i].ToString();    
                }
                Debug.Log("Relaxed IK: " + jaStr + "]");

                foreach (Transform gripper in grippers) {
                    gripper.localPosition = new Vector3(0, 0, 0);
                    gripper.localRotation = new Quaternion(0, 0, 0, 1);
                }
            } else {
                foreach (TransformGizmo gizmo in gizmos)
                    gizmo.enabled = false;
            }
        }

        // Some hard code to transform the coordinate system
        private Vector3 TransformUnityToRvizPos(Vector3 pos)
        {
            // ur5
            // return new Vector3(pos.z, -pos.y, pos.x);
            // iiwa7
            // return new Vector3(pos.y, -pos.x, pos.z);
            // sawyer ?
            return new Vector3(pos.x, pos.y, pos.z);
        }

        private Quaternion TransformUnityToRvizRot(Quaternion quat) 
        {
            // ur5
            // return new Quaternion(quat.z, -quat.y, quat.x, quat.w);
            // iiwa7
            // return new Quaternion(quat.y, -quat.x, quat.z, quat.w);
            // sawyer ?
            return new Quaternion(quat.x, quat.y, quat.z, quat.w);
        }
    }

    // [CustomEditor(typeof(JointAnglesUnity))]
    // [CanEditMultipleObjects]
    // public class JointAnglesUnityEditor : Editor
    // {
    //     SerializedProperty JointAnglesSubscriber;
    //     SerializedProperty isRadians;
    //     SerializedProperty usingRelaxedIKAngles;
    //     SerializedProperty robotLimbs;
    //     SerializedProperty angles;
    //     SerializedProperty joints;
    //     bool showJoints =  false;

    //     void OnEnable(){
    //         JointAnglesSubscriber = serializedObject.FindProperty("sub");
    //         usingRelaxedIKAngles = serializedObject.FindProperty("usingRelaxedIKAngles");
    //         isRadians = serializedObject.FindProperty("isRadians");
    //         robotLimbs = serializedObject.FindProperty("robotLimbs");
    //         angles = serializedObject.FindProperty("angles");
    //         joints = serializedObject.FindProperty("joints");
    //     }

    //     public override void OnInspectorGUI(){
    //         serializedObject.UpdateIfRequiredOrScript();
    //         EditorGUILayout.PropertyField(JointAnglesSubscriber, new GUIContent("Joint Angles Subscriber"));

    //         EditorGUILayout.Space();

    //         EditorGUILayout.PropertyField(robotLimbs, true);

    //         EditorGUILayout.Space();

    //         EditorGUILayout.PropertyField(usingRelaxedIKAngles);

    //         //Hide manual angles and is radians if using relaxedIk angles
    //         if (!usingRelaxedIKAngles.boolValue)
    //         {
    //             EditorGUI.indentLevel++;
    //             EditorGUILayout.PropertyField(isRadians);
    //             EditorGUILayout.PropertyField(angles, true);
    //             EditorGUI.indentLevel--;
    //         }

    //         EditorGUILayout.Space();

    //         showJoints = EditorGUILayout.Toggle("Show Joints", showJoints);

    //         if (showJoints)
    //         {
    //             EditorGUI.indentLevel++;
    //             GUI.enabled = false;
    //             EditorGUILayout.PropertyField(joints, true);
    //             GUI.enabled = true;
    //             EditorGUI.indentLevel--;
    //         }

    //         serializedObject.ApplyModifiedProperties();                     
    //     }
    // }
}