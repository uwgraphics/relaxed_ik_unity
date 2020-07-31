using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private float lookSpeedH = 2f;
        [SerializeField]
        private float lookSpeedV = 2f;
        [SerializeField]
        private float zoomSpeed = 2f;
        [SerializeField]
        private float dragSpeed = 0.03f;

        private float yaw = 0f;
        private float pitch = 0f;

        // Start is called before the first frame update
        private void Start()
        {
            RelaxedIKUnity r = FindObjectOfType(typeof(RelaxedIKUnity)) as RelaxedIKUnity;
            if (r.EELinks.Count > 1) {
                transform.position = new Vector3(0, 1.2f, 1.2f);
            }
            Transform target = r.transform.Find("ViewCenter");
            if (target) {
                transform.LookAt(target);
            } else {
                transform.LookAt(r.transform);
            }

            // Initialize the correct initial rotation
            yaw = transform.eulerAngles.y;
            pitch = transform.eulerAngles.x;
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                // Look around with Left Mouse
                if (Input.GetMouseButton(0))
                {
                    yaw += lookSpeedH * Input.GetAxis("Mouse X");
                    pitch -= lookSpeedV * Input.GetAxis("Mouse Y");

                    transform.eulerAngles = new Vector3(pitch, yaw, 0f);
                }

                // Drag camera around with Right Mouse
                if (Input.GetMouseButton(1))
                {
                    transform.Translate(-Input.GetAxis("Mouse X") * dragSpeed, -Input.GetAxis("Mouse Y") * dragSpeed, 0);
                }

                //Zoom in and out with Mouse Wheel
                transform.Translate(0, 0, Input.GetAxis("Mouse ScrollWheel") * zoomSpeed);
            }
        }
    }
}