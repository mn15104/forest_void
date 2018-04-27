﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This is the event manager of which a new instance is attached to every area. Once the player enters the area a set of events will be triggered. 
public class EventManager : MonoBehaviour {

    public enum Stage
    {
        Intro,Stage1,Stage2,Stage3,GameOverStage 
    }
   
    public enum Location
    {
        Chapel,Forest,Crypt,ToolShed, Caravan, Generator, Bridge
    }
    public void SetGameTimeFromStage(Stage stage)
    {
        switch (stage)
        {
            case Stage.Stage1:
                GameTimerSeconds = StageTimes[0];
                break;
            case Stage.Stage2:
                GameTimerSeconds = StageTimes[1];
                break;
            case Stage.Stage3:
                GameTimerSeconds = StageTimes[2];
                break;
        }
    }
    private float[] StageTimes = { 50f, 200f, 420f, 480f };
    public float GameTimerSeconds = 0f;

    public TriggerEvent2 TextTriggerEvent = new TriggerEvent2();

    public TriggerEvent BridgeCrossedEvent = new TriggerEvent();
    public TriggerEvent ChapelBackDoorHandEvent = new TriggerEvent();
    public TriggerEvent ToolShedDoorHandEvent = new TriggerEvent();
    public TriggerEvent CaravanTriggerEvent = new TriggerEvent();
    public TriggerEvent GeneratorZoneTriggerEvent = new TriggerEvent();
    public TriggerEvent StructureZoneTriggerEvent = new TriggerEvent();

    public NotifyEvent<bool> NotifyBlueKeyPickup = new NotifyEvent<bool>();
    public NotifyEvent<bool> NotifyYellowKeyPickup = new NotifyEvent<bool>();
    public NotifyEvent<float> NotifyHeartRate = new NotifyEvent<float>();
    public NotifyEvent<Stage> NotifyStage = new NotifyEvent<Stage>();
    public NotifyEvent<Location> NotifyLocation = new NotifyEvent<Location>();
    public NotifyEvent<bool> NotifyRunStamina = new NotifyEvent<bool>();
    public NotifyEvent<bool> NotifyTorchPressed = new NotifyEvent<bool>();

    private VoidSystem voidSys;
    public Stage debugChangeStage;
    private bool debugForceStageChange = false;
    private Stage currentStage;
    private Location currentLocation;

    public GameObject player;
    public GameObject monster;
    public GameObject playerSpawnPoint;
    public bool StoryMode = false;
    private float startNotifyingHeartRate = 20;
    private float NotifyHeartRateInterval = 3;

    private void OnEnable()
    {
        voidSys = FindObjectOfType<VoidSystem>();
        currentStage = Stage.Intro;
        debugChangeStage = Stage.Intro;
        currentLocation = Location.Forest;
        voidSys.NotifyStage.NotifyEventOccurred += SetStage;
        StructureZoneTriggerEvent.TriggerEnterEvent += SetStructureLocation;
        StructureZoneTriggerEvent.TriggerExitEvent += SetForestLocation;
    }


    public void Start()
    {
        InvokeRepeating("PassHeartRate", startNotifyingHeartRate, NotifyHeartRateInterval);
        NotifyStage.Notify(currentStage);

        // An in-development system to simulate real game chronological behaviour/events
        if (StoryMode)
        {
            player.transform.position = playerSpawnPoint.transform.position;
           // player.transform.rotation = playerSpawnPoint.transform.rotation;
        }

    }

    void Update()
    {
        GameTimerSeconds += Time.deltaTime;
        if(debugChangeStage != currentStage && !debugForceStageChange)
        {
            debugForceStageChange = true;
            ForceStageChange();
        }
      
    }
    
    public float getPlayerHeartrate()
    {
        return player.GetComponentInChildren<Heartbeat>().m_Heartbeat;
    }

    public int getCurrentKeyCount()
    {
        return player.GetComponent<Inventory>().keys.Count;
        
    }
    void ForceStageChange()
    {
        voidSys.ResetVoidToStage(debugChangeStage);
        SetGameTimeFromStage(debugChangeStage);
    }
    // VoidSystem notifies EventManager of stage, EventManager notifies every other necessary script
    void SetStage(Stage t_stage)
    {
        currentStage = t_stage;
        debugChangeStage = t_stage;
        if (debugForceStageChange)
        {
            SetGameTimeFromStage(t_stage);
        }
        NotifyStage.Notify(currentStage);
        debugForceStageChange = false;
    }

    void PassHeartRate()
    {
        float currentHeartRate = getPlayerHeartrate();
        NotifyHeartRate.Notify(currentHeartRate);
    }
    
    void SetStructureLocation(GameObject gameObject)
    {
        Debug.Log(gameObject.GetComponent<StructureZone>().location);
        currentLocation = gameObject.GetComponent<StructureZone>().location;
        NotifyLocation.Notify(currentLocation);
    } 

    void SetForestLocation(GameObject gameObject)
    {
        Debug.Log("Exited " + currentLocation);
        currentLocation = Location.Forest;
        NotifyLocation.Notify(currentLocation);
    }
    public Location GetLocation()
    {
        return currentLocation;
    }
    public float GetGameTime()
    {
        return GameTimerSeconds;
    }
    public bool GetIsInForest()
    {
        return currentLocation == Location.Forest;
    }
}
