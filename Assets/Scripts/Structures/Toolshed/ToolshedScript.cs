using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolshedScript : MonoBehaviour {

    public bool blueKeyTrigger = false;
    private bool eventTriggered = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(blueKeyTrigger && !eventTriggered)
        {
            eventTriggered = true;
            foreach(Rigidbody rig in GetComponentsInChildren<Rigidbody>())
            {
                rig.isKinematic = false;
            }
        }
	}
}
