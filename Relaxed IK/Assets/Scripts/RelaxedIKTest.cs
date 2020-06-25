using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelaxedIKTest : MonoBehaviour
{
    
    public double[] pos_test = new double[] { 0.015, 0.015, 0.015 };
    public double[] quat_test = new double[] { 0.0, 0.0, 0.0, 1.0 };

    private Opt xopt;

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private unsafe void Update()
    {
        xopt = RelaxedIK.runUnity(pos_test, pos_test.Length, quat_test, quat_test.Length);
        double[] ja = new double[xopt.length];
        string ja_str = "";
        for (int i = 0; i < xopt.length; i++)
        {
            ja[i] = xopt.data[i];
            if (i == 0)
            {
                ja_str += "[" + ja[i].ToString();
            }
            else
            {
                ja_str += ", " + ja[i].ToString();
            }
        }
        Debug.Log(ja_str + "]");
    }
}
