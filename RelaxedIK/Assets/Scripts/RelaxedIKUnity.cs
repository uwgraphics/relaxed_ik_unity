using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class RelaxedIKUnity : MonoBehaviour
    {
        public bool enableRelaxedIK;
        public List<GameObject> robotLinks;
        public Transform gripper;
        /*public double[] posTest = new double[] { 0.015, 0.015, 0.015 };
        public double[] quatTest = new double[] { 0.0, 0.0, 0.0, 1.0 };*/

        private List<Joint> joints;
        private List<Quaternion> baseRotations;
        private Vector3 gripperPos;
        private Quaternion gripperQuat;
        // private Transform root;
        private Opt xopt;

        // Start is called before the first frame update
        void Start()
        {
            joints = new List<Joint>();
            baseRotations = new List<Quaternion>();
            // root = transform.root;

            gripperPos = gripper.localPosition;
            gripperQuat = gripper.localRotation;

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
        }

        private unsafe void Update()
        {
            if (enableRelaxedIK) 
            {
                gripperPos += TransformUnityToRvizPos(gripper.localPosition);
                gripperQuat *= TransformUnityToRvizRot(gripper.localRotation);
                
                // Debug.Log(gripperPos);
                // Debug.Log(gripperQuat);
                double[] posArr = new double[] { gripperPos.x, gripperPos.y, gripperPos.z };
                double[] quatArr = new double[] { gripperQuat.x, gripperQuat.y, gripperQuat.z, gripperQuat.w };
                
                xopt = RelaxedIKLoader.runUnity(posArr, posArr.Length, quatArr, quatArr.Length);
                
                string jaStr = "";
                for (int i = 0; i < xopt.length; i++)
                {
                    float angle = Mathf.Rad2Deg * (float) xopt.data[i];
                    Vector3 axis = angle * joints[i].axis;
                    robotLinks[i].transform.localEulerAngles = (baseRotations[i] * Quaternion.Euler(axis)).eulerAngles;
                    jaStr += i == 0 ? "[" + xopt.data[i].ToString() : ", " + xopt.data[i].ToString();    
                }
                Debug.Log(jaStr + "]");

                gripper.localPosition = new Vector3(0, 0, 0);
                gripper.localRotation = new Quaternion(0, 0, 0, 1);
            }
        }

        // Some hard code to transform the coordinate system
        private Vector3 TransformUnityToRvizPos(Vector3 pos)
        {
            return new Vector3(pos.z, -pos.y, pos.x);
        }

        private Quaternion TransformUnityToRvizRot(Quaternion quat) 
        {
            return new Quaternion(quat.z, -quat.y, quat.x, quat.w);
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