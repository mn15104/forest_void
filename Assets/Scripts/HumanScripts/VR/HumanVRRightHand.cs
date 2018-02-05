using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HumanVRRightHand : MonoBehaviour {

    public delegate void HumanLightEmitter(bool on);
    public static event HumanLightEmitter OnHumanLightEmission;

    private Flashlight flashlight;
    private int id = 1;

    private void OnEnable()
    {
        flashlight = GetComponentInChildren<Flashlight>();
    }

    // Use this for initialization
    void Start () {

    }
	
	//Update is called once per frame
	void Update () {
        if (SixenseInput.Controllers[id].GetButtonDown(SixenseButtons.TRIGGER))
        {
            flashlight.Switch(gameObject);
            if (flashlight.m_FlashlightActive)
            {
                OnHumanLightEmission(true);
            }
            else
            {
                OnHumanLightEmission(false);
            }
        }
    }
    
}
