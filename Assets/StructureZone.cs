using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureZone : MonoBehaviour {

    private EventManager eventManager;

    public void Awake()
    {
       
        eventManager = FindObjectOfType<EventManager>();
    }

    public void OnEnable()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == eventManager.player.GetComponent<Collider>())
        {
            eventManager.StructureZoneTriggerEvent.TriggerEnter(other);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == eventManager.player.GetComponent<Collider>())
        {
            eventManager.StructureZoneTriggerEvent.TriggerExit(other);

        }
    }

}
