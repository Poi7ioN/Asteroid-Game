using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script handles the disable call for objects like particle effects,bullets. (these objects are pooled)
public class DisableScript : MonoBehaviour
{
    public void CallDisable(float delay)   
    {
        Invoke("disableEffect", delay);  //Invoke a disable method with a float delay 
    }

    private void disableEffect()
    {
        gameObject.SetActive(false);   //Disable the gameobject
    }
}
