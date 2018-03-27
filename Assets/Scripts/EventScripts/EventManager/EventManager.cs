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

    private float[] StageTimes = { 0f, 120f, 600f, 720f };
    private float GameTimerSeconds = 0f; 

    public TriggerEvent TextTriggerEvent;
    public TriggerEvent BridgeCrossedEvent;
    public TriggerEvent ChapelBackDoorHandEvent;
    public TriggerEvent CaravanTriggerEvent;
    public TriggerEvent GeneratorZoneTriggerEvent;
    public TriggerEvent StructureZoneTriggerEvent;

    public NotifyEvent<float> NotifyHeartRate;
    public NotifyEvent<Stage> NotifyStage;
    public NotifyEvent<Location> NotifyLocation;
    public NotifyEvent<bool> NotifyRunStamina;
    public NotifyEvent<bool> NotifyTorchPressed;


    private Stage currentStage;
    private Location currentLocation;

    public GameObject player;
    public GameObject monster;

    private float startNotifyingHeartRate = 20;
    private float NotifyHeartRateInterval = 3;
    
   
    public void Awake()
    {
       
        NotifyHeartRate = new NotifyEvent<float>();
        NotifyStage = new NotifyEvent<Stage>();
        NotifyLocation = new NotifyEvent<Location>();
        NotifyRunStamina = new NotifyEvent<bool>();
        NotifyTorchPressed = new NotifyEvent<bool>();

        BridgeCrossedEvent = new TriggerEvent();
        ChapelBackDoorHandEvent = new TriggerEvent();
        CaravanTriggerEvent = new TriggerEvent();
        GeneratorZoneTriggerEvent = new TriggerEvent();
        TextTriggerEvent = new TriggerEvent();
        StructureZoneTriggerEvent = new TriggerEvent();

        currentStage = Stage.Stage0;
        currentLocation = Location.Forest;
    
        EventManagerSubscriptions();
    }

    public void Start()
    {
        //E.G Notify heart rate to all listeners, after 20sec -> every 3 sec
        InvokeRepeating("PassHeartRate", startNotifyingHeartRate, NotifyHeartRateInterval); 
    }

    void EventManagerSubscriptions()
    {
        StructureZoneTriggerEvent.TriggerEnterEvent += SetStructureLocation;
        StructureZoneTriggerEvent.TriggerExitEvent += SetForestLocation;
    }

    public float getPlayerHeartrate()
    {
        return player.GetComponentInChildren<Heartbeat>().m_Heartbeat;
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
                    NotifyStage.Notify(currentStage);
                }
                break;
            case Stage.Stage1:
                if (GameTimerSeconds > StageTimes[2])
                {
                    currentStage = Stage.Stage2;
                    NotifyStage.Notify(currentStage);
                }
                break;
            case Stage.Stage2:
                if (GameTimerSeconds > StageTimes[3])
                {
                    currentStage = Stage.Stage3;
                    NotifyStage.Notify(currentStage);
                }
                break;
        }
    }

    void PassHeartRate()
    {
        float currentHeartRate = getPlayerHeartrate();
        NotifyHeartRate.Notify(currentHeartRate);
    }


    void Update()
    {
        GameTimerSeconds += Time.deltaTime;
        UpdateStage();
    }


    void SetStructureLocation(GameObject gameObject)
    {
        currentLocation = gameObject.GetComponent<StructureZone>().location;
        NotifyLocation.Notify(currentLocation);
    } 

    void SetForestLocation(GameObject gameObject)
    {
        currentLocation = Location.Forest;
        NotifyLocation.Notify(currentLocation);
    }


}
