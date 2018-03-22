using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyRetrieveZone : MonoBehaviour {

    // Use this for initialization
    private EventManager eventManager; 

    void Awake () {
        //lHandAnchor = GameObject.Find("OVRHuman/OPRCameraRig/TrackingSpace/LeftHandAnchor");
        eventManager = FindObjectOfType<EventManager>();
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            eventManager.GeneratorZoneTriggerEvent.TriggerEnter(other);
            Debug.Log("Entered generator area");
        }
   
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) { 
        
            eventManager.GeneratorZoneTriggerEvent.TriggerExit(other);
            Debug.Log("Outside generator area");
        }
    }
}
