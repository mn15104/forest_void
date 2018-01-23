﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour {

    private Light m_Light;
    private bool m_FlashlightActive = false;

    void OnEnable()
    {
        HumanEventManager.OnUseItem += Switch;
    }
    void OnDisable()
    {
        HumanEventManager.OnUseItem -= Switch;
    }

    // Use this for initialization
    void Start () {
        m_Light = GetComponentInChildren<Light>();
        m_Light.intensity = 0;
	}
	
    public void FV_Rotate(Vector3 rotation, float angle)
    {
        transform.Rotate(rotation, angle);
    }

    public void Switch(GameObject t)
    {
        if (m_FlashlightActive && (GetComponentInParent<HumanController>()  || GetComponentInParent<HumanVRController>()))
        {
            m_Light.intensity = 0;
            m_FlashlightActive = false;
        }
        else if (!m_FlashlightActive && (GetComponentInParent<HumanController>() || GetComponentInParent<HumanVRController>()))
        {
            m_Light.intensity = 40;
            m_FlashlightActive = true;
        }
    }
    

	// Update is called once per frame
	void Update () {
		
	}
}
