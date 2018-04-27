using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricFenceLight : MonoBehaviour {

    public GameObject goLight;
    public GameObject stopLight;
    protected EventManager eventManager;


    // Use this for initialization
    void Start () {
        stopLight.GetComponent<Light>().enabled = true;
        goLight.GetComponent<Light>().enabled = false;
        eventManager = FindObjectOfType<EventManager>();
        eventManager.NotifyAllKeysInserted.NotifyEventOccurred += switchToGoLight;
    }

    void switchToGoLight(bool value)
    {
        stopLight.GetComponent<Light>().enabled = false;
        goLight.GetComponent<Light>().enabled = true;
    }   

	
	// Update is called once per frame
	void Update () {
		
	}
}
