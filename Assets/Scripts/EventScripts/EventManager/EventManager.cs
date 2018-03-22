using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This is the event manager of which a new instance is attached to every area. Once the player enters the area a set of events will be triggered. 
public class EventManager : MonoBehaviour {

    public enum Stage
    {
        Stage0,Stage1,Stage2,Stage3,GameOverStage 
    }
   
 public enum Location
    {
        Chapel,Forest,Crypt,ToolShed, Caravan, Generator
    }


    public float[] StageTimes = { 0f, 120f, 600f, 720f };


    public float GameTimerSeconds = 0f; 

    public TriggerEvent TextTriggerEvent;
    public TriggerEvent BridgeCrossedEvent;
    public TriggerEvent ChapelBackDoorHandEvent;
    public TriggerEvent CaravanTriggerEvent;
    public TriggerEvent GeneratorZoneTriggerEvent;
    public TriggerEvent StructureZoneTriggerEvent;

    public Stage currentStage;
    public Location currentLocation;
    public GameObject player;
    public GameObject monster;
    
   
    public void Awake()
    {
     
        BridgeCrossedEvent = new TriggerEvent();
        ChapelBackDoorHandEvent = new TriggerEvent();
        CaravanTriggerEvent = new TriggerEvent();
        GeneratorZoneTriggerEvent = new TriggerEvent();
        TextTriggerEvent = new TriggerEvent(); 


        StructureZoneTriggerEvent = new TriggerEvent();


        currentStage = Stage.Stage0;
        currentLocation = Location.Forest;
        Debug.Log(getCurrentKeyCount());

        EventManagerSubscriptions();
    }

    void EventManagerSubscriptions()
    {
        StructureZoneTriggerEvent.TriggerEnterEvent += DeactivateMonster;
        StructureZoneTriggerEvent.TriggerEnterEvent += SetStructureLocation;
        StructureZoneTriggerEvent.TriggerExitEvent += ActivateMonster;
        StructureZoneTriggerEvent.TriggerExitEvent += SetForestLocation;
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

    void UpdateStage()
    {
        switch (currentStage)
        {
            case Stage.Stage0:
                if (GameTimerSeconds > StageTimes[1])
                {
                    currentStage = Stage.Stage1;
                }
                break;
            case Stage.Stage1:
                if (GameTimerSeconds > StageTimes[2])
                {
                    currentStage = Stage.Stage2;
                }
                break;
            case Stage.Stage2:
                if (GameTimerSeconds > StageTimes[3])
                {
                    currentStage = Stage.Stage3;
                }
                break;
        }
    }


    void Update()
    {
        GameTimerSeconds += Time.deltaTime;
        UpdateStage();
  
    }


    void DeactivateMonster(GameObject colliderObject)
    {
        Debug.Log("Deactivate Monster ");
    }

    void ActivateMonster(GameObject colliderObject)
    {

    }

    void SetStructureLocation(GameObject gameObject)
    {
        currentLocation = gameObject.GetComponent<StructureZone>().location;

    } 

    void SetForestLocation(GameObject gameObject)
    {
        currentLocation = Location.Forest;
    }


}
