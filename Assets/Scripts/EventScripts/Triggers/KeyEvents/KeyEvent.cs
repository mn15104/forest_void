using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyEvent : EventTrigger {


    private void OnTriggerEnter(Collider other)
    {
        // Only if the player collider hits the trigger
        if (other == GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>())
        {
            Debug.Log("Collider" + other);
            Debug.Log("I think there's a key near here: " + gameObject);
            gameObject.SetActive(false); // TODO: MOVE TO CONTROLLER
        }
    }
}
