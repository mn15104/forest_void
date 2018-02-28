using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyRetrieveZone : MonoBehaviour {

    // Use this for initialization
    public GameObject lHandAnchor;

    void Awake () {
        //lHandAnchor = GameObject.Find("OVRHuman/OPRCameraRig/TrackingSpace/LeftHandAnchor");
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            lHandAnchor.GetComponent<RetreiveKey>().inGeneratorZone = true;
            Debug.Log("Entered generator area");
        }
   
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            lHandAnchor.GetComponent<RetreiveKey>().inGeneratorZone = false;
            Debug.Log("Outside generator area");
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
