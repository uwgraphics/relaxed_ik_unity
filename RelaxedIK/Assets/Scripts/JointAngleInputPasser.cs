using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RosSharp.RosBridgeClient
{
    public class JointAngleInputPasser : MonoBehaviour
    {
        public GameObject panel;
        public GameObject hGroupPrefab;
        public GameObject textPrefab;
        public GameObject sliderPrefab;
        public float maxJointVelocity;

        private List<JoyAxisJointTransformWriter> writers;
        [SerializeField]
        private List<Slider> sliders;

        // Start is called before the first frame update
        void Start()
        {
            writers = new List<JoyAxisJointTransformWriter>();
            sliders = new List<Slider>();

            RelaxedIKUnity r = FindObjectOfType(typeof(RelaxedIKUnity)) as RelaxedIKUnity;
            foreach (GameObject link in r.robotLinks)
            {
                JoyAxisJointTransformWriter writer = link.AddComponent(typeof(JoyAxisJointTransformWriter)) as JoyAxisJointTransformWriter;
                writer.MaxVelocity = maxJointVelocity;
                writers.Add(writer);

                GameObject hGroup = Instantiate(hGroupPrefab, panel.transform) as GameObject;
                hGroup.name = "panel_" + link.name;
                GameObject textObj = Instantiate(textPrefab, hGroup.transform) as GameObject;
                textObj.name = "text_" + link.name;
                textObj.GetComponent<Text>().text = link.name;

                GameObject sliderObj = Instantiate(sliderPrefab, hGroup.transform) as GameObject;
                sliderObj.name = "slider_" + link.name;
                Slider slider = sliderObj.GetComponent<Slider>();
                sliderObj.GetComponentInChildren<Text>().text = slider.value.ToString("0.0");
                slider.onValueChanged.AddListener(
                    delegate {
                        sliderObj.GetComponentInChildren<Text>().text = slider.value.ToString("0.0");
                        writer.Write(slider.value);
                    }
                );
                sliders.Add(slider);
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void SliderOnInput() {

        }
    }
}
