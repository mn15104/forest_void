using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEvent {

    // Use this for initialization
    public delegate void Events(GameObject Object);
    public event Events TriggerEnterEvent;
    public event Events TriggerExitEvent;

    public void TriggerEnter(GameObject gameObject)
    {
        TriggerEnterEvent(gameObject);
    }
    public void TriggerExit(GameObject gameObject)
    {
        TriggerExitEvent(gameObject);
    }

}
