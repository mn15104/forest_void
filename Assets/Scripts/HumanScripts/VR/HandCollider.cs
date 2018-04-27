using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCollider : MonoBehaviour {

    private Collider handSphereCollider;
    public List<Collider> fenceColliders;
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
        eventManager.ElectricFieldEvent.TriggerEnterEvent += EnableHandCollider;
        eventManager.ChapelBackDoorHandEvent.TriggerExitEvent += DisableHandCollider;
        eventManager.ToolShedDoorHandEvent.TriggerExitEvent += DisableHandCollider;
        eventManager.ElectricFieldEvent.TriggerExitEvent += DisableHandCollider;
    }


    void EnableHandCollider(GameObject gameObject)
    {
        handSphereCollider.enabled = true;
    }

    void DisableHandCollider(GameObject gameObject)
    {
        handSphereCollider.enabled = false;
    }


    private void Update()
    {
        if(handSphereCollider.enabled)
        {
            foreach (Collider c in fenceColliders)
            {
                if (handSphereCollider.bounds.Intersects(c.bounds))
                {
                    GetComponentInParent<OculusHaptics>().Vibrate(VibrationForce.Hard);
                }
            }
        }
    } 
}
