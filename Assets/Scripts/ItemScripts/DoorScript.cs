using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour {

    
    BoxCollider frontDoor;
    HumanController interactingPlayer;
    Rigidbody body;
    HingeJoint joint;


    void OnEnable()
    {
        foreach (Transform child in GetComponents<Transform>())
        {
            if (child.gameObject.tag == "HandleGlow")
            {
                child.gameObject.SetActive(false);
            }
        }
        HumanEventManager.OnInteract += OpenDoor;
    }
    void OnDisable()
    {
        HumanEventManager.OnInteract -= OpenDoor;
    }
    
    // Use this for initialization
    void Start () {
        frontDoor = GetComponent<BoxCollider>();
        body = GetComponentInParent<Rigidbody>();
        joint = gameObject.GetComponent<HingeJoint>();
    }
    
    private void OpenDoor(GameObject other)
    {
        HumanController player = other.GetComponent<HumanController>();
        body.mass = 1;
        body.AddForceAtPosition(transform.forward * 2, new Vector3(-4, 0, 0));
        //body.AddForce(-transform.forward, ForceMode.VelocityChange);
    }

   
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<HumanController>())
        {
            interactingPlayer = other.gameObject.GetComponent<HumanController>();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<HumanController>())
        {
            interactingPlayer = null;
        }
    }


    // Update is called once per frame
    void Update ()
    {
	}
}
