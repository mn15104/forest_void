using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyGrabbable : OVRGrabbable {

    // Use this for initialization
    public GameObject target;
    public float speed;
    private GameObject human;
    public bool hasBeenInserted = false;
    public bool hasBeenCollected = false;
    public GameObject light;

    protected override void Start()
    {
        base.Start();
        human = GameObject.FindGameObjectWithTag("Player");
        speed = 0.7f;
        light.GetComponent<Light>().enabled = false;
    }

    public override void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        transform.GetComponent<Collider>().enabled = true;
        m_grabbedBy = hand;
        m_grabbedCollider = grabPoint;
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        hasBeenCollected = true;
    }

    public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = m_grabbedKinematic;
        rb.velocity = new Vector3(0,0,0);
        rb.angularVelocity = new Vector3(0, 0, 0);
        m_grabbedBy = null;
        m_grabbedCollider = null;
        checkInserted(); 
    }

    public void checkInserted()
    {
        //Need to check if verticle
        if (target.GetComponent<Collider>().bounds.Intersects(transform.GetComponent<Collider>().bounds))
        {
            hasBeenInserted = true;
            transform.GetComponent<Rigidbody>().useGravity = false;
            transform.GetComponent<Collider>().enabled = false;
            transform.GetComponent<Rigidbody>().isKinematic = true;
        }
       
    }

    public IEnumerator InsertionAnimation()
    {
        Vector3 finalKeyPosition = new Vector3(target.transform.position.x - 0.025f, target.transform.position.y , target.transform.position.z);
        Debug.Log("inserted key");
        while (Vector3.Distance(transform.position, finalKeyPosition) > 0.0001f)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, finalKeyPosition, step);
            Debug.Log("Stuck in loop");
            yield return 1;

        }
        
        //Wait to do rotation
        yield return new WaitForSeconds(0.5f);

        Quaternion startingRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(-180,0,0);
        float originalRotationTime = 0.3f;
        float currentTime = 0;
        while(currentTime < originalRotationTime)
        {
            currentTime += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startingRotation, targetRotation, currentTime/originalRotationTime);
            yield return new WaitForEndOfFrame();
        }
    }

    void Update () {
        Debug.Log("Has key been inserted" + hasBeenInserted); 
        if (hasBeenInserted)
        {
            human.GetComponent<Inventory>().removeKeyFromInventory(gameObject);
            //Animate insertion;
            light.GetComponent<Light>().enabled = true;
            StartCoroutine(InsertionAnimation());
            hasBeenInserted = false;

        }

    }




}
