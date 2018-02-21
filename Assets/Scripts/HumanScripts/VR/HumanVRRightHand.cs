using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HumanVRRightHand : MonoBehaviour {

    public delegate void HumanLightEmitter(bool on);
    public static event HumanLightEmitter OnHumanLightEmission;

    private Flashlight flashlight;
    private int id = 1;
    private float timer;
    private void OnEnable()
    {
        flashlight = GetComponentInChildren<Flashlight>();
    }

    // Use this for initialization
    void Start () {
        timer = 0;
    }
	
	//Update is called once per frame
	void Update () {
        //if (SixenseInput.Controllers[id].GetButtonDown(SixenseButtons.TRIGGER))
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.9 && timer == 0)
        {
            flashlight.Switch(gameObject);
            timer = 0.3f;
            if (flashlight.m_FlashlightActive)
            {
                OnHumanLightEmission(true);
            }
            else
            {
                OnHumanLightEmission(false);
            }
        }
        timer = Mathf.Max(timer - Time.deltaTime, 0);

    }
    
}
