using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaircaseTeleport : MonoBehaviour {
    public GameObject destinationTeleport;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
       
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<HumanController>() != null)
        {
            collision.gameObject.transform.localPosition = destinationTeleport.transform.localPosition;
            Debug.Log(collision.gameObject.transform.localPosition + destinationTeleport.transform.localPosition);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<HumanController>() != null)
        {
            Vector3 rotate = other.gameObject.transform.rotation.eulerAngles;
            other.gameObject.transform.SetPositionAndRotation(destinationTeleport.transform.position, Quaternion.Euler(rotate) );
        }
       // Quaternion.Euler(new Vector3(rotate.x, rotate.y + 180, rotate.z))
    }
}
