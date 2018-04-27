using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricFenceDoor : MonoBehaviour {

    private EventManager eventManager;

    // Use this for initialization
    void Start () {
        eventManager = FindObjectOfType<EventManager>();
        eventManager.NotifyAllKeysInserted.NotifyEventOccurred += RotateDoorOpen;
    }

    void RotateDoorOpen(bool value)
    {
        transform.Rotate(new Vector3(0, -67.394f, 0));
    }


	
	// Update is called once per frame
	void Update () {
		
	}
}
