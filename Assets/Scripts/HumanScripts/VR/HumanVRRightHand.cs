using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HumanVRRightHand : MonoBehaviour
{

    public delegate void HumanLightEmitter(bool on);
    public static event HumanLightEmitter OnHumanLightEmission;

    private Flashlight flashlight;
    private EventManager eventManager;
    private bool heldDown;
    private bool quickPress = false;
    private int id = 1;
    private float timer;

    private void Awake()
    {
        eventManager = FindObjectOfType<EventManager>();
    }

    private void OnEnable()
    {
        flashlight = GetComponentInChildren<Flashlight>();
        
    }

    // Use this for initialization
    void Start()
    {
        timer = 0;
        heldDown = false;
    }

    void Update()
    {
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.9 && timer == 0 && !heldDown)
        {
          
            flashlight.Switch(gameObject);
            quickPress = true;
            // if it is being held down then just keep it on until let go
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
        if (quickPress && !(heldDown) && (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.9) && timer < 0.02f)
        {
            heldDown = true;
            quickPress = false;
        }
        if (heldDown && OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) < 0.3)
        {
            if (flashlight.GetComponentInChildren<Light>().intensity > 0)
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
            heldDown = false;
            Debug.Log("helddown" + heldDown);
        }


    }

}