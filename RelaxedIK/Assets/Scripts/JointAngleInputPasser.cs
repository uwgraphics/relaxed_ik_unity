using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RosSharp.RosBridgeClient
{
    public class InputPasser
    {
        public Slider slider;
        public Text text;
        public JointStateWriter writer;
        public bool updated;
    }

    public class JointAngleInputPasser : MonoBehaviour
    {
        public GameObject hGroupPrefab;
        public GameObject vGroupPrefab;
        public GameObject labelPrefab;
        public GameObject sliderPrefab;
        public GameObject buttonPrefab;
        // public GameObject togglePrefab;

        private Transform contentPanel;
        private List<InputPasser> passers;
        private bool randomized;
        private bool zeroed;
        private RelaxedIKUnity r;

        // Start is called before the first frame update
        void Start()
        {
            contentPanel =  transform.Find("Content");

            GameObject enableRIKBtn = Instantiate(buttonPrefab, transform.Find("Toggle")) as GameObject;
            enableRIKBtn.name = "toggle_enable_RIK";
            Text enableRIKBtnText = enableRIKBtn.GetComponentInChildren<Text>();
            enableRIKBtnText.text = "Enable Relaxed IK";
            enableRIKBtn.GetComponent<Button>().onClick.AddListener(
                delegate {
                    if (r.enableRelaxedIK) {
                        r.enableRelaxedIK = false;
                        enableRIKBtnText.text = "Enable Relaxed IK";
                        SetContentPanelActive();
                        transform.Find("Header").Find("MinimizeButton").gameObject.SetActive(true);
                        transform.Find("Header").Find("MaximizeButton").gameObject.SetActive(true);
                    } else {
                        r.enableRelaxedIK = true;
                        enableRIKBtnText.text = "Disable Relaxed IK";
                        SetContentPanelInactive();
                        transform.Find("Header").Find("MinimizeButton").gameObject.SetActive(false);
                        transform.Find("Header").Find("MaximizeButton").gameObject.SetActive(false);
                    }
                }
            );

            passers = new List<InputPasser>();

            r = FindObjectOfType(typeof(RelaxedIKUnity)) as RelaxedIKUnity;
            for (int i = 0; i < r.robotLinks.Count; i++)
            {
                GameObject link = r.robotLinks[i];
                InputPasser passer = new InputPasser();

                GameObject vGroup = Instantiate(vGroupPrefab, contentPanel.Find("Sliders")) as GameObject;
                vGroup.name = "vertical_group_" + i;

                GameObject hGroup = Instantiate(hGroupPrefab, vGroup.transform) as GameObject;
                hGroup.name = "horizontal_group_" + i;
                GameObject textObj = Instantiate(labelPrefab, hGroup.transform) as GameObject;
                textObj.name = "text_" + i;
                textObj.GetComponent<Text>().text = link.name;

                GameObject sliderObj = Instantiate(sliderPrefab, vGroup.transform) as GameObject;
                sliderObj.name = "slider_" + i;
                passer.slider = sliderObj.GetComponent<Slider>();

                HingeJointLimitsManager jointManager = link.GetComponent<HingeJointLimitsManager>();
                passer.slider.value = jointManager.AngleActual * Mathf.Deg2Rad;
                passer.slider.maxValue = jointManager.LargeAngleLimitMax * Mathf.Deg2Rad;
                passer.slider.minValue = jointManager.LargeAngleLimitMin * Mathf.Deg2Rad;
                passer.slider.onValueChanged.AddListener(delegate {passer.updated = true;});

                GameObject labelObj = Instantiate(labelPrefab, hGroup.transform) as GameObject;
                labelObj.name = "label_" + i;
                passer.text = labelObj.GetComponent<Text>();
                passer.text.text = passer.slider.value.ToString("0.0");

                passer.writer = link.AddComponent(typeof(JointStateWriter)) as JointStateWriter;

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
            foreach (InputPasser passer in passers) 
            {
                if (passer.updated) {
                    passer.text.text = passer.slider.value.ToString("0.0");
                    passer.writer.Write(passer.slider.value);
                    passer.updated = false;
                }
            }

            if (randomized) {
                foreach (InputPasser passer in passers) 
                {
                    passer.slider.value = Random.Range(passer.slider.minValue, passer.slider.maxValue); 
                    passer.text.text = passer.slider.value.ToString("0.0");
                    passer.writer.Write(passer.slider.value);
                }
                randomized = false;
            }

            if (zeroed) {
                foreach (InputPasser passer in passers) 
                {
                    passer.slider.value = 0f; 
                    passer.text.text = passer.slider.value.ToString("0.0");
                    passer.writer.Write(passer.slider.value);
                }
                zeroed = false;
            }
        }

        public void SetContentPanelActive() {
            contentPanel.gameObject.SetActive(true);
        }

        public void SetContentPanelInactive() {
            contentPanel.gameObject.SetActive(false);
        }
    }
}
