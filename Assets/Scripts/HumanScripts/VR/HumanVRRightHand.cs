using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HumanVRRightHand : MonoBehaviour {
    
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
        }
    }

  
}
