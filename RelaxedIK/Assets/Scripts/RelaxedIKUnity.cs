using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class RelaxedIKUnity : MonoBehaviour
    {
        public List<GameObject> robotLinks;
        public double[] posTest = new double[] { 0.015, 0.015, 0.015 };
        public double[] quatTest = new double[] { 0.0, 0.0, 0.0, 1.0 };

        [SerializeField]
        private List<Joint> joints;
        private List<Quaternion> baseRotations;
        // private Transform root;
        private Opt xopt;
        // private List<float> ja;

        // Start is called before the first frame update
        void Start()
        {
            joints = new List<Joint>();
            baseRotations = new List<Quaternion>();
            // root = transform.root;

            foreach (GameObject link in robotLinks)
            {
                Joint joint = link.GetComponent<Joint>();
                if (joint.axis.y == 1)
                {
                    joint.axis = new Vector3(0,-1,0);
                }
                joints.Add(joint);
                baseRotations.Add(link.transform.localRotation);
            }            
        }

        private unsafe void FixedUpdate()
        {
            xopt = RelaxedIKLoader.runUnity(posTest, posTest.Length, quatTest, quatTest.Length);
            
            string ja_str = "";
            for (int i = 0; i < xopt.length; i++)
            {
                float angle = Mathf.Rad2Deg * (float) xopt.data[i];
                Vector3 axis = angle * joints[i].axis;
                robotLinks[i].transform.localEulerAngles = (baseRotations[i] * Quaternion.Euler(axis)).eulerAngles;
                ja_str += i == 0 ? "[" + xopt.data[i].ToString() : ", " + xopt.data[i].ToString();
            }
            Debug.Log(ja_str + "]");
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