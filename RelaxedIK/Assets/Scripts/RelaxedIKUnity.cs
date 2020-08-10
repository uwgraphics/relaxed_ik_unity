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
        // public GameObject poseGoalPrefab;
        public Opt xopt;

        private List<Transform> grippers;
        private List<Joint> joints;
        private List<Quaternion> baseRotations;
        private List<TransformGizmo> gizmos;
        // private bool initialized;

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
                GameObject gripper = Instantiate(gripperPrefab, EELink) as GameObject;
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
                    Transform gripper = grippers[i];
                    Transform poseGoal = grippers[i].Find("PoseGoal");
                    poseGoal.Translate(TransformUnityToRviz(gripper.localPosition));
                    Vector3 quatTemp = TransformUnityToRviz(new Vector3(gripper.localRotation.x, gripper.localRotation.y, gripper.localRotation.z));
                    poseGoal.localRotation *= new Quaternion(quatTemp.x, quatTemp.y, quatTemp.z, gripper.localRotation.w);

                    posArr[3*i] = poseGoal.localPosition.x;
                    posArr[3*i+1] = poseGoal.localPosition.y;
                    posArr[3*i+2] = poseGoal.localPosition.z;
                    quatArr[4*i] = poseGoal.localRotation.x;
                    quatArr[4*i+1] = poseGoal.localRotation.y;
                    quatArr[4*i+2] = poseGoal.localRotation.z;
                    quatArr[4*i+3] = poseGoal.localRotation.w;

                    if (i > 0) {
                        posStr += ", ";
                        quatStr += ", ";
                    }

                    posStr += poseGoal.localPosition.ToString("F3");
                    quatStr += poseGoal.localRotation.ToString("F3");
                }
                // Debug.Log(posStr);
                // Debug.Log(quatStr);
                
                xopt = RelaxedIKLoader.solve(posArr, posArr.Length, quatArr, quatArr.Length);
                
                string jaStr = "";
                for (int i = 0; i < xopt.length; i++)
                {
                    float angle = Mathf.Rad2Deg * (float) xopt.data[i];
                    Vector3 axis = angle * joints[i].axis;
                    robotLinks[i].transform.localEulerAngles = (baseRotations[i] * Quaternion.Euler(axis)).eulerAngles;
                    jaStr += i == 0 ? "[" + xopt.data[i].ToString() : ", " + xopt.data[i].ToString();    
                }
                Debug.Log("Relaxed IK: " + jaStr + "]");

                for (int i = 0; i < grippers.Count; i++) {
                    grippers[i].localPosition = new Vector3(0, 0, 0);
                    grippers[i].localRotation = new Quaternion(0, 0, 0, 1);
                }

                // if (!initialized) {
                //     initialized = true;
                //     for (int i = 0; i < grippers.Count; i++) {
                //         poseGoals[i].position = EELinks[i].position;
                //         poseGoals[i].rotation = EELinks[i].rotation;
                //     }
                // }
            } else {
                foreach (TransformGizmo gizmo in gizmos)
                    gizmo.enabled = false;
            }
        }

        // Some hard code to transform the coordinate system
        private Vector3 TransformUnityToRviz(Vector3 v)
        {
            if (name == "baxter") {
                return new Vector3(v.y, v.z, -v.x);
            } else if (name == "hubo") {
                return new Vector3(v.y, v.x, -v.z);
            } else if (name == "iiwa7") {
                return new Vector3(v.y, v.z, -v.x);
            } else if (name == "jaco7") {
                return new Vector3(v.y, v.z, -v.x);
            } else if (name == "panda") {
                return new Vector3(v.x, -v.z, -v.y);
            } else if (name == "sawyer") {
                return new Vector3(v.y, v.x, v.z);
            } else if (name == "ur5") {
                return new Vector3(-v.z, -v.y, v.x);
            } else if (name == "yumi") {
                return new Vector3(v.x, v.y, v.z);
            } else {
                return new Vector3(v.x, v.y, v.z);
            }
        }
    }
}