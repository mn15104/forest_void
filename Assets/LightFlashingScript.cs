using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlashingScript : MonoBehaviour {

    public Light redLight;
    private float startTime;
    private float currentTime;
    private bool looped;

	// Use this for initialization
	void Start () {
        looped = false;
		
	}
	
	// Update is called once per frame
	void Update () {

        currentTime = Time.time;

        if (!looped && currentTime > startTime + 1.5) {
            looped = true;
            redLight.intensity = 2f;
        }
        if (looped && currentTime > startTime + 2)
        {
            looped = false;
            redLight.intensity = 0f;
            startTime = Time.time;
        }

	}
}
