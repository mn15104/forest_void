using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This is the event manager of which a new instance is attached to every area. Once the player enters the area a set of events will be triggered. 
public class EventManager : MonoBehaviour {

    public TriggerEvent FenceTextTriggerEvent;
    public TriggerEvent BridgeCrossedEvent;
    public TriggerEvent ChapelTriggerEvent;

    //public static event Events ChapelEvents;
    //public static event Events CaravanEvents;

    public GameObject player;

    public void Awake()
    {
        FenceTextTriggerEvent = new TriggerEvent();
        BridgeCrossedEvent = new TriggerEvent();
        ChapelTriggerEvent = new TriggerEvent();
    }

    public void Start()
    {
        Debug.Log(ChapelTriggerEvent);
    }

  

}
