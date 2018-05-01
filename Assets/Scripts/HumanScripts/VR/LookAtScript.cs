using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtScript : MonoBehaviour {

    public TrackingSpaceController trackingSpace;
    
    public bool LookBack = false;
    public float rotateSpeed = 5f;

	// Use this for initialization
	void Start () {

    }

    // Update is called once per frame
    void Update () {
        if(LookBack)
        {
            trackingSpace.VRLookAT(transform, rotateSpeed);
            LookBack = false;
        }
	}
}
