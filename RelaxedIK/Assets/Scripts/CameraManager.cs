using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform target;
    // Start is called before the first frame update
    private void Start()
    {
        transform.LookAt(target);
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
}
