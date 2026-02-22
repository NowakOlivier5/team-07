using UnityEngine;
using System.Collections;

public class FlashLight : MonoBehaviour {
    private Light myLight;
      
      
    void Start ()
    {
        myLight = GetComponent<Light>();
        myLight.enabled = false; //flashlight starts off
    }
      
    void Update ()
    {
        if(Input.GetKeyUp(KeyCode.T)) //press T
        {
            myLight.enabled = !myLight.enabled; //toggle flashlight
        }
    }
}