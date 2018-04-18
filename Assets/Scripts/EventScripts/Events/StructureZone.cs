using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureZone : MonoBehaviour {

    public EventManager.Location location;


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
            Debug.Log("Structure Event triggered");
            eventManager.StructureZoneTriggerEvent.TriggerEnter(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == eventManager.player.GetComponent<Collider>())
        {
            eventManager.StructureZoneTriggerEvent.TriggerExit(gameObject);

        }
    }

}
