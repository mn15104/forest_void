using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricFieldEvent : MonoBehaviour {

    protected EventManager eventManager;
    protected TriggerEvent trigger;

    // Use this for initialization
    public virtual void Awake()
    {
        eventManager = FindObjectOfType<EventManager>();
        trigger = eventManager.ElectricFieldEvent;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == eventManager.player)
        {
            trigger.TriggerEnter(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == eventManager.player)
        {
            trigger.TriggerExit(other.gameObject);
        }

    }
}
