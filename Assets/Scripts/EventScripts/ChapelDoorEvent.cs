using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapelDoorEvent : MonoBehaviour
{

    protected EventManager eventManager;
    protected TriggerEvent trigger;

    // Use this for initialization
    public virtual void Awake()
    {
        eventManager = FindObjectOfType<EventManager>();
        trigger = eventManager.ChapelBackDoorHandEvent;
    }

    private void OnTriggerEnter(Collider other)
    {
        trigger.TriggerEnter(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        trigger.TriggerExit(other.gameObject);

    }
}
