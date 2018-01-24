using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerActivate : MonoBehaviour {

	public GameObject player;
	public GameObject trigger;

	private Collider myCollider;
	private Collider triggerCollider;

    // Use this for initialization
    void Start()
    {
        myCollider = GetComponent<Collider>();
        triggerCollider = trigger.GetComponent<Collider>();
    }
	

	void OnTriggerEnter(Collider other) {
        Debug.Log("in trigger");    
		if(other.tag == player.tag){
            //rend.material.SetColor("_Color", Color.green);
            Debug.Log("triggered");
			//Enable the trigger
			triggerCollider.enabled = true;
			myCollider.enabled = false;
		}
	}
}
