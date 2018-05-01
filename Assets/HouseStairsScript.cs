
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HouseStairsScript : MonoBehaviour
{


    protected EventManager eventManager;

    private bool triggered;

    public GameObject flashlight;
    public GameObject nextTrigger;

    public Light MainLight;
    public Light RimLight;

    private bool flashlightTrickery;
    private bool flashlightTrickery2;
    private bool flashlightTrickery3;
    private bool flashlightTrickery4;
    private bool flashlightTrickery5;
    private float flashlightStartTime;
    private float flashlightCurrentTime;

    // Use this for initialization
    void Start()
    {
        flashlightTrickery = false;
        flashlightTrickery2 = false;
        flashlightTrickery3 = false;
        flashlightTrickery4 = false;
        flashlightTrickery5 = false;
        nextTrigger.GetComponent<BoxCollider>().enabled = false;
        flashlightStartTime = 10000f;

    }

    private void Update()
    {

        flashlightCurrentTime = Time.time;
        if (triggered && !flashlightTrickery)
        {
            flashlightStartTime = Time.time;
            //flick off
            flashlight.GetComponent<Flashlight>().enabled = false;
            MainLight.intensity = 0f;
            RimLight.intensity = 0f;
            flashlight.GetComponentInChildren<Light>().intensity = 0f;
            flashlightTrickery = true;
        }
        if ((triggered && !flashlightTrickery2) && flashlightCurrentTime > flashlightStartTime + 0.2f)
        {
            ///flick on
            MainLight.intensity = 0.1f;
            RimLight.intensity = 0.2f;
            flashlight.GetComponentInChildren<Light>().intensity = 1.0f;
            flashlightTrickery2 = true;
        }
        if ((triggered && !flashlightTrickery3) && flashlightCurrentTime > flashlightStartTime + 0.9f)
        {
            //flick off
            MainLight.intensity = 0f;
            RimLight.intensity = 0f;
            flashlight.GetComponentInChildren<Light>().intensity = 0f;
            flashlightTrickery3 = true;
        }
        if ((triggered && !flashlightTrickery4) && flashlightCurrentTime > flashlightStartTime + 1.2f)
        {
            //flick on
            MainLight.intensity = 0.1f;
            RimLight.intensity = 0.2f;
            flashlight.GetComponentInChildren<Light>().intensity = 1.0f;
            flashlightTrickery4 = true;
        }
        if ((triggered && !flashlightTrickery5) && flashlightCurrentTime > flashlightStartTime + 1.8f)
        {
            //flick off
            MainLight.intensity = 0f;
            RimLight.intensity = 0f;
            flashlight.GetComponentInChildren<Light>().intensity = 0.0f;
            flashlightTrickery5 = true;
            nextTrigger.GetComponent<BoxCollider>().enabled = true;
        }

    }


    private void OnTriggerEnter(Collider other)
    {



        if (!triggered)
        {
           
            triggered = true;
        }
    }


}
