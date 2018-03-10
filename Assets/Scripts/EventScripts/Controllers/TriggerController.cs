using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerController : MonoBehaviour
{

    public delegate void TriggerEvents();
    public static event TriggerEvents OnTriggerActivate;

    //Initial trigger
    GameObject bridgeTrigger;

    // Use this for initialization
    void Start()
    {
        bridgeTrigger = FindObjectOfType<BridgeEvent>().gameObject;

        //Disable all triggers except bridge trigger
        foreach (EventTrigger trigger in FindObjectsOfType<EventTrigger>())
        {
            trigger.gameObject.SetActive(false);
        }
        bridgeTrigger.SetActive(true);
    }

    public void EnableTrigger()
    {
        OnTriggerActivate();
    }

}
