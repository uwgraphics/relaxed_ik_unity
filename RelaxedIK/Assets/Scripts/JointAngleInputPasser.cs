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
        public GameObject textPrefab;
        public GameObject labelPrefab;
        public GameObject sliderPrefab;

        private List<InputPasser> passers;

        // Start is called before the first frame update
        void Start()
        {
            passers = new List<InputPasser>();

            RelaxedIKUnity r = FindObjectOfType(typeof(RelaxedIKUnity)) as RelaxedIKUnity;
            foreach (GameObject link in r.robotLinks)
            {
                InputPasser passer = new InputPasser();

                GameObject hGroup = Instantiate(hGroupPrefab, transform) as GameObject;
                hGroup.name = "panel_" + link.name;
                GameObject textObj = Instantiate(textPrefab, hGroup.transform) as GameObject;
                textObj.name = "text_" + link.name;
                textObj.GetComponent<Text>().text = link.name;

                GameObject sliderObj = Instantiate(sliderPrefab, hGroup.transform) as GameObject;
                sliderObj.name = "slider_" + link.name;
                passer.slider = sliderObj.GetComponent<Slider>();

                HingeJointLimitsManager jointManager = link.GetComponent<HingeJointLimitsManager>();
                passer.slider.value = jointManager.AngleActual * Mathf.Deg2Rad;
                passer.slider.maxValue = jointManager.LargeAngleLimitMax * Mathf.Deg2Rad;
                passer.slider.minValue = jointManager.LargeAngleLimitMin * Mathf.Deg2Rad;
                passer.slider.onValueChanged.AddListener(delegate {passer.updated = true;});

                GameObject labelObj = Instantiate(labelPrefab, hGroup.transform) as GameObject;
                labelObj.name = "label_" + link.name;
                passer.text = labelObj.GetComponent<Text>();
                passer.text.text = passer.slider.value.ToString("0.0");

                passer.writer = link.AddComponent(typeof(JointStateWriter)) as JointStateWriter;

                passers.Add(passer);
            }
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
        }
    }
}
