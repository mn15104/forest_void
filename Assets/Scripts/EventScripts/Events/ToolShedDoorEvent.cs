using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolShedDoorEvent : MonoBehaviour
{

    protected EventManager eventManager;
    protected TriggerEvent trigger;

    // Use this for initialization
    public virtual void Awake()
    {
        eventManager = FindObjectOfType<EventManager>();
        trigger = eventManager.ToolShedDoorHandEvent;
     }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == eventManager.player)
        {
            Debug.Log("Trigger entered at tool shed door");
            trigger.TriggerEnter(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == eventManager.player)
        {
            Debug.Log("Trigger entered at chapel door");
            trigger.TriggerExit(other.gameObject);
        }

    }
}
