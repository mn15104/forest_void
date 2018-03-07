using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerController : MonoBehaviour {

    public enum Triggers { HOUSE, GENERATOR };
    GameObject enableTrigger;
    GameObject disableTrigger;

    GameObject houseTrigger;
    GameObject bridgeTrigger;
    GameObject generatorTrigger;
    KeyEvent[] keyEvents;

    // Use this for initialization
    void Start() {

        //Grab an instance of each event trigger, can't "find" it later
        bridgeTrigger = FindObjectOfType<BridgeEvent>().gameObject;
        houseTrigger = FindObjectOfType<HouseEvent>().gameObject;
        generatorTrigger = FindObjectOfType<GeneratorEvent>().gameObject;
        keyEvents = FindObjectsOfType<KeyEvent>();

        //Disable all triggers except bridge trigger
        foreach (EventTrigger trigger in FindObjectsOfType<EventTrigger>())
        {
            trigger.gameObject.SetActive(false);
        }
        bridgeTrigger.SetActive(true);
    }

    //Enables trigger and disables the previous one
    public void EnableTrigger(Triggers trigger)
    {
        switch (trigger)
        {
            case Triggers.HOUSE:
                enableTrigger = houseTrigger;
                disableTrigger = bridgeTrigger;
                break;
            case Triggers.GENERATOR:
                enableTrigger = generatorTrigger;
                disableTrigger = houseTrigger;
                break;
            default:
                enableTrigger = null;
                disableTrigger = null;
                break;
        }
        enableTrigger.SetActive(true);
        disableTrigger.GetComponent<Collider>().enabled = false;
        Debug.Log("Event trigger " + enableTrigger + " enabled");
        Debug.Log("Event trigger " + disableTrigger + " disabled");
    }

    public void EnableKeyEvents()
    {
        foreach(KeyEvent keyEvent in keyEvents)
        {
            keyEvent.gameObject.SetActive(true);
        }
        //Disable the generator trigger
        //generatorTrigger.SetActive(false);
        bridgeTrigger.SetActive(false);
    }
}
