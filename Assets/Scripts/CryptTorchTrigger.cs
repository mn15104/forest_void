using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CryptTorchTrigger : MonoBehaviour {

    public GameObject flashlight;


	// Use this for initialization
	void Start () {
		flashlight = gameObject.GetComponentInChildren<Light>().gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        { 

            if (flashlight.activeInHierarchy)
            {
                Debug.Log("light active");
                flashlight.SetActive(false);
            }
            else
            {
                flashlight.SetActive(true);
            }
        }
      
    }
}
