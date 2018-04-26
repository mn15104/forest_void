using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutexScript : MonoBehaviour {

    public bool mutext; // best pun 2018

	// Use this for initialization
	void Start () {
        mutext = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void getMutex()
    {
        mutext = true;
    }

    public void releaseMutex()
    {
        mutext = false;
    }

    public bool isUnlocked()
    {
        return !mutext;
    }
}
