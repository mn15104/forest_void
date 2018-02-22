using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyIn : MonoBehaviour {

    public GameObject key;
    private bool keyInserted = false;

	// Use this for initialization
	void Start () {
        Collider keyCollider = key.GetComponent<Collider>();
	}
	
	// Update is called once per frame
	void Update () {
        if (keyInserted)
        {
            //transform.Rotate(Vector3.up * Time.deltaTime * 45);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(-90, -90, 0), Time.deltaTime);
            key.transform.rotation = Quaternion.Lerp(key.transform.rotation, Quaternion.Euler(180, 0, 0), Time.deltaTime);
           // key.GetComponent<KeyGrabbable>().enabled = false; // WARNING: Might cause script to be deactivated for other keys???
        }
    }

    private void OnTriggerEnter(Collider keyCollider)
    {
        Debug.Log("Key inserted");
        keyInserted = true;
    }
}
