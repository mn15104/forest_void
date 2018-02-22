using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyGrabbable : OVRGrabbable {

    // Use this for initialization
    public GameObject target;
    public GameObject area;
    public float speed;
    private bool grabEnded = false;

    void Start () {
        base.Start();
    }

    public override void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        grabEnded = false; 
        m_grabbedBy = hand;
        m_grabbedCollider = grabPoint;
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
    }

    public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = m_grabbedKinematic;
        rb.velocity = new Vector3(0,0,0);
        rb.angularVelocity = new Vector3(0, 0, 0);
        m_grabbedBy = null;
        m_grabbedCollider = null;
        grabEnded = true;
    }

    // Update is called once per frame
    void Update () {
        if (grabEnded && area.GetComponent<Collider>().bounds.Contains(transform.position))
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);
        }
        
	}
}
