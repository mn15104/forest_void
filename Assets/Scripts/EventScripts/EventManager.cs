using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This is the event manager of which a new instance is attached to every area. Once the player enters the area a set of events will be triggered. 
public class EventManager : MonoBehaviour {

    public enum Stage
    {
        Stage0,Stage1,Stage2,Stage3,GameOverStage 
    }

    public TriggerEvent FenceTextTriggerEvent;
    public TriggerEvent BridgeCrossedEvent;
    public TriggerEvent ChapelTriggerEvent;
    public TriggerEvent CaravanTriggerEvent;

    public Stage currentStage; 
    public GameObject player;
    public GameObject monster;

   
    public void Awake()
    {
        FenceTextTriggerEvent = new TriggerEvent();
        BridgeCrossedEvent = new TriggerEvent();
        ChapelTriggerEvent = new TriggerEvent();
        CaravanTriggerEvent = new TriggerEvent();
        currentStage = Stage.Stage0;
        Debug.Log(getCurrentKeyCount());
    }

    public float getPlayerHeartrate()
    {
        return player.GetComponentInChildren<Heartbeat>().m_Heartbeat;
    }

    public bool getIsPlayerRunning()
    {
        return player.GetComponent<OVRPlayerController>().CurrentRunningEnergy < 10;
    }

    public int getCurrentKeyCount()
    {
        return player.GetComponent<Inventory>().keys.Count;
    }





}
