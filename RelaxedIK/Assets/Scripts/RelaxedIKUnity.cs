using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using RuntimeGizmos;

namespace RosSharp.RosBridgeClient
{
    public class RelaxedIKUnity : MonoBehaviour
    {
        public bool enableRelaxedIK;
        public List<GameObject> robotLinks;
        public List<Transform> EELinks;
        public GameObject gripperPrefab;
        public Opt xopt;

        private List<Transform> grippers;
        private List<Joint> joints;
        private List<Quaternion> baseRotations;
        private List<TransformGizmo> gizmos;
        private bool initialized;

        // Start is called before the first frame update
        private void Start()
        {
            string[] paths = {Directory.GetCurrentDirectory(), "..", "relaxed_ik_core", "config", "loaded_robot"};
            string fullPath = Path.Combine(paths);
            // Debug.Log(fullPath);
            File.WriteAllText(fullPath, name + "_info.yaml");

            grippers = new List<Transform>();
            joints = new List<Joint>();
            baseRotations = new List<Quaternion>();

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

            foreach (Transform EELink in EELinks) {
                GameObject gripper = Instantiate(gripperPrefab, transform) as GameObject;
                grippers.Add(gripper.transform);
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
                string posStr = "Goal position: ";
                string quatStr = "Goal rotation: ";
                for (int i = 0; i < grippers.Count; i++) {
                    Transform poseGoal = grippers[i].Find("Collision");
                    Vector3 gripperPos = TransformUnityToRviz(poseGoal.localPosition);
                    Vector3 quatTemp = TransformUnityToRviz(new Vector3(poseGoal.localRotation.x, poseGoal.localRotation.y, poseGoal.localRotation.z));
                    Quaternion gripperQuat = new Quaternion(quatTemp.x, quatTemp.y, quatTemp.z, poseGoal.localRotation.w);

                    posArr[3*i] = gripperPos.x;
                    posArr[3*i+1] = gripperPos.y;
                    posArr[3*i+2] = gripperPos.z;
                    quatArr[4*i] = gripperQuat.x;
                    quatArr[4*i+1] = gripperQuat.y;
                    quatArr[4*i+2] = gripperQuat.z;
                    quatArr[4*i+3] = gripperQuat.w;

                    if (i > 0) {
                        posStr += ", ";
                        quatStr += ", ";
                    }
                    posStr += gripperPos.ToString("F3");
                    quatStr += gripperQuat.ToString("F3");
                }
                Debug.Log(posStr);
                Debug.Log(quatStr);
                
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

                if (!initialized) {
                    initialized = true;
                    for (int i = 0; i < grippers.Count; i++) {
                        grippers[i].position = EELinks[i].position;
                        grippers[i].rotation = EELinks[i].rotation;
                    }
                }
            } else {
                foreach (TransformGizmo gizmo in gizmos)
                    gizmo.enabled = false;
            }
        }

        // Some hard code to transform the coordinate system
        private Vector3 TransformUnityToRviz(Vector3 v)
        {
            if (name == "baxter") {
                return new Vector3(v.z, -v.y, -v.x);
            } else if (name == "iiwa7") {
                return new Vector3(v.y, -v.z, -v.x);
            } else if (name == "jaco7") {
                return new Vector3(v.y, -v.z, -v.x);
            } else if (name == "panda") {
                return new Vector3(v.y, -v.x, -v.z);
            } else if (name == "sawyer") {
                return new Vector3(v.y, v.x, v.z);
            } else if (name == "ur5") {
                return new Vector3(v.z, -v.y, v.x);
            } else {
                return new Vector3(v.x, v.y, v.z);
            }
        }
    }
}