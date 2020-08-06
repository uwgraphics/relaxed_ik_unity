using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RosSharp.RosBridgeClient
{
    public class InputPasser
    {
        public GameObject link;
        public string type;
        public Vector3 basePos;
        public Quaternion baseRot;
        public Slider slider;
        public Text text;
        public bool updated;
    }

    public class JointAngleInputPasser : MonoBehaviour
    {
        public GameObject hGroupPrefab;
        public GameObject vGroupPrefab;
        public GameObject sliderGroupPrefab;
        public GameObject labelPrefab;
        public GameObject sliderPrefab;
        public GameObject buttonPrefab;

        private Transform contentPanel;
        private GameObject enableRIKBtn;
        private List<InputPasser> passers;
        private RelaxedIKUnity r;
        private bool randomized;
        private bool zeroed;

        // Start is called before the first frame update
        void Start()
        {
            r = FindObjectOfType(typeof(RelaxedIKUnity)) as RelaxedIKUnity;
            if (!r) {
                gameObject.SetActive(false);
                Debug.Log("Please select one robot to play with by enabling the corresponding gameobject!");
                return;
            }
            contentPanel =  transform.Find("Content");
            passers = new List<InputPasser>();

            enableRIKBtn = Instantiate(buttonPrefab, transform.Find("Toggle")) as GameObject;
            enableRIKBtn.name = "toggle_enable_RIK";
            if (r.enableRelaxedIK) {
                EnableRIKListener();
            } else {
                DisableRIKListener();
            }
            enableRIKBtn.GetComponent<Button>().onClick.AddListener(
                delegate {
                    if (r.enableRelaxedIK) {
                        r.enableRelaxedIK = false;
                        DisableRIKListener();
                    } else {
                        r.enableRelaxedIK = true;
                        EnableRIKListener();
                    }
                }
            );

            for (int i = 0; i < r.EELinks.Count; i++) {
                GameObject sliderGroup = Instantiate(sliderGroupPrefab, contentPanel.Find("Sliders")) as GameObject;
                sliderGroup.name = "slider_group_" + i;
            }

            for (int i = 0; i < r.robotLinks.Count; i++)
            {
                GameObject link = r.robotLinks[i];
                InputPasser passer = new InputPasser();
                passer.link = link;
                passer.basePos = link.transform.localPosition;
                passer.baseRot = link.transform.localRotation;

                int groupIndex = i / (r.robotLinks.Count / r.EELinks.Count);
                Transform targetGroup = contentPanel.Find("Sliders").Find("slider_group_" + groupIndex.ToString());

                GameObject vGroup = Instantiate(vGroupPrefab, targetGroup) as GameObject;
                vGroup.name = "vertical_group_" + i;

                GameObject hGroup = Instantiate(hGroupPrefab, vGroup.transform) as GameObject;
                hGroup.name = "horizontal_group_" + i;
                GameObject textObj = Instantiate(labelPrefab, hGroup.transform) as GameObject;
                textObj.name = "text_" + i;
                textObj.GetComponent<Text>().text = link.name;

                GameObject sliderObj = Instantiate(sliderPrefab, vGroup.transform) as GameObject;
                sliderObj.name = "slider_" + i;
                passer.slider = sliderObj.GetComponent<Slider>();

                HingeJointLimitsManager rJointManager = link.GetComponent<HingeJointLimitsManager>();
                if (rJointManager) {
                    passer.type = "revolute";
                    passer.slider.value = rJointManager.AngleActual * Mathf.Deg2Rad;
                    passer.slider.maxValue = rJointManager.LargeAngleLimitMin * -1.0f * Mathf.Deg2Rad;
                    passer.slider.minValue = rJointManager.LargeAngleLimitMax * -1.0f * Mathf.Deg2Rad;
                } else {
                    PrismaticJointLimitsManager pJointManager = link.GetComponent<PrismaticJointLimitsManager>();
                    if (pJointManager) {
                        passer.type = "prismatic";
                        passer.slider.maxValue = pJointManager.PositionLimitMax;
                        passer.slider.minValue = pJointManager.PositionLimitMin;
                    } else {
                        passer.type = "unknown";
                    }
                }
                
                passer.slider.onValueChanged.AddListener(delegate {passer.updated = true;});

                GameObject labelObj = Instantiate(labelPrefab, hGroup.transform) as GameObject;
                labelObj.name = "label_" + i;
                passer.text = labelObj.GetComponent<Text>();
                passer.text.text = passer.slider.value.ToString("0.00");

                passers.Add(passer);
            }

            Transform btnsPanel = contentPanel.Find("Buttons");

            GameObject randomizeBtn = Instantiate(buttonPrefab, btnsPanel) as GameObject;
            randomizeBtn.name = "button_randomize";
            randomizeBtn.GetComponentInChildren<Text>().text = "Randomize";
            randomizeBtn.GetComponent<Button>().onClick.AddListener(delegate {randomized = true;});

            GameObject zeroBtn = Instantiate(buttonPrefab, btnsPanel) as GameObject;
            randomizeBtn.name = "button_zero";
            zeroBtn.GetComponentInChildren<Text>().text = "Center";
            zeroBtn.GetComponent<Button>().onClick.AddListener(delegate {zeroed = true;});
        }

        // Update is called once per frame
        void Update()
        {
            if (!r) return;
            string jaStr = "";
            for (int i = 0; i < passers.Count; i++)
            {
                if (passers[i].updated) {
                    UpdateJointAngle(passers[i]);
                    passers[i].updated = false;
                }
                jaStr += i == 0 ? "[" + passers[i].slider.value.ToString() : ", " + passers[i].slider.value.ToString();  
            }
            // Debug.Log("Joint Angle Writer: " + jaStr + "]");

            if (randomized) {
                foreach (InputPasser passer in passers) 
                {
                    passer.slider.value = Random.Range(passer.slider.minValue, passer.slider.maxValue); 
                    UpdateJointAngle(passer);
                }
                randomized = false;
            }

            if (zeroed) {
                foreach (InputPasser passer in passers) 
                {
                    passer.slider.value = 0f; 
                    UpdateJointAngle(passer);
                }
                zeroed = false;
            }
        }

        private void UpdateJointAngle(InputPasser passer) {
            passer.text.text = passer.slider.value.ToString("0.00");
            if (passer.type == "revolute") {
                float angle = Mathf.Rad2Deg * passer.slider.value;
                Vector3 axis = angle * passer.link.GetComponent<Joint>().axis;
                passer.link.transform.localEulerAngles = (passer.baseRot * Quaternion.Euler(axis)).eulerAngles;
            } else if (passer.type == "prismatic") {
                Vector3 axis = passer.slider.value * passer.link.GetComponent<ConfigurableJoint>().axis;
                passer.link.transform.localPosition = passer.basePos + axis;
            }
        }

        private void EnableRIKListener() {
            enableRIKBtn.GetComponentInChildren<Text>().text = "Disable Relaxed IK";
            SetContentPanelInactive();
            transform.Find("Header").Find("MinimizeButton").gameObject.SetActive(false);
            transform.Find("Header").Find("MaximizeButton").gameObject.SetActive(false);
        }

        private void DisableRIKListener() {
            enableRIKBtn.GetComponentInChildren<Text>().text = "Enable Relaxed IK";
            SetContentPanelActive();
            transform.Find("Header").Find("MinimizeButton").gameObject.SetActive(true);
            transform.Find("Header").Find("MaximizeButton").gameObject.SetActive(true);
        }

        public unsafe void SetContentPanelActive() {
            for (int i = 0; i < passers.Count; i++) 
            {
                passers[i].slider.value = (float) r.xopt.data[i];
                passers[i].text.text = passers[i].slider.value.ToString("0.0");
                passers[i].updated = false;
            }
            contentPanel.gameObject.SetActive(true);
        }

        public void SetContentPanelInactive() {
            contentPanel.gameObject.SetActive(false);
        }
    }
}
