using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCollider : MonoBehaviour {

    private Collider handSphereCollider;
    private EventManager eventManager;
	// Use this for initialization

	void Awake () {
        eventManager = FindObjectOfType<EventManager>();
        handSphereCollider = GetComponent<Collider>();
        handSphereCollider.enabled = false;
    }

    private void Start()
    {
        eventManager.ChapelBackDoorHandEvent.TriggerEnterEvent += EnableHandCollider;
        eventManager.ToolShedDoorHandEvent.TriggerEnterEvent += EnableHandCollider;
        eventManager.ChapelBackDoorHandEvent.TriggerExitEvent += DisableHandCollider;
        eventManager.ToolShedDoorHandEvent.TriggerExitEvent += DisableHandCollider;
    }


    void EnableHandCollider(GameObject gameObject)
    {
        
        handSphereCollider.enabled = true;
    }

    void DisableHandCollider(GameObject gameObject)
    {
        handSphereCollider.enabled = false;
    } 
}
