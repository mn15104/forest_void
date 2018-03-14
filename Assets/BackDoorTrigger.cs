using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackDoorTrigger : MonoBehaviour {

    private GameObject handCollider;

    private void Start()
    {
        handCollider = GameObject.FindWithTag("HandCollider");
        handCollider.SetActive(false);

    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            handCollider.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            handCollider.SetActive(false);
        }
    }
}
