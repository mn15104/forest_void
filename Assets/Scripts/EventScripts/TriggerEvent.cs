using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEvent : ScriptableObject {

    // Use this for initialization
    public delegate void Events(GameObject Object);
    public event Events TriggerEnterEvent;
    public event Events TriggerExitEvent;



    public void TriggerEnter(Collider trigger)
    {
        TriggerEnterEvent(trigger.gameObject);
    }
    public void TriggerExit(Collider trigger)
    {
        TriggerExitEvent(trigger.gameObject);
    }

}
