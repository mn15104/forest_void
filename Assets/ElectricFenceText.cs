using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElectricFenceText : MonoBehaviour {

    private EventManager eventManager;

    // Use this for initialization
    void Start () {
        eventManager = FindObjectOfType<EventManager>();
        eventManager.NotifyAllKeysInserted.NotifyEventOccurred += TextDisappear;
    }

    void TextDisappear(bool value) {
        GetComponent<Text>().enabled = false;
    }


	
	// Update is called once per frame
	void Update () {
		
	}
}
