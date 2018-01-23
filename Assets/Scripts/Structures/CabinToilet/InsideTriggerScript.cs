using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsideTriggerScript : MonoBehaviour {

    
    bool eventTriggered = false;
    float eventTimer = 0f;
    AudioSource aud;
	// Use this for initialization
	void Start () {
        aud = GetComponentInChildren<AudioSource>();
        aud.Stop();
        aud.loop = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(eventTimer > 2f && eventTriggered == false)
        {
            eventTimer = 0f;
            eventTriggered = true;
            aud.Play();
        }
	}

    private void OnTriggerStay(Collider other)
    {
        if ((other.gameObject.GetComponent<HumanController>() || other.gameObject.transform.root.GetComponent<HumanVRController>()) && !eventTriggered)
        {
            eventTimer += Time.deltaTime;
        }
    }
}
