using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverGlitching : MonoBehaviour {

    public GlitchEffect glitchEffect;
    public float glitchChangeSpeed;
    public bool startingGlitching = false;

	// Use this for initialization
	void Start () {

		
	}
	
	// Update is called once per frame
	void Update () {
		if (startingGlitching)
        {
            glitchEffect.enabled = true;
            var lerpAmount = Mathf.Lerp(glitchEffect.intensity, 0.65f, Time.deltaTime * glitchChangeSpeed);
            glitchEffect.intensity = lerpAmount;
            glitchEffect.flipIntensity = lerpAmount;
            glitchEffect.colorIntensity = lerpAmount;

        }
	}

    public void endGlitching()
    {
        glitchEffect.enabled = false;
    }

    public void startGlitching()
    {
        startingGlitching = true;
    }
}
