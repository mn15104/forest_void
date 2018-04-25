using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorColliderScript : MonoBehaviour {

    public bool entered;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "player")
        {
            entered = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "player")
        {
            entered = false;
        }
    }
}
